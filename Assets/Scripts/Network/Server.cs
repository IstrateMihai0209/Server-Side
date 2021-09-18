using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;

public class Server
{
    public static int maxPlayers { get; private set; }

    public static int Port { get; private set; }

    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

    /// <summary>
    /// The server decides which packet method to call based on the packet ID that it receives
    /// </summary>
    /// <param name="_fromClient">The ID of the client</param>
    /// <param name="_packet">The packet that is sent</param>
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    /// <summary>
    /// Used to keep track of the packet handler
    /// </summary>
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;
    private static UdpClient udpListener; //manages all UDP communication for the server

    ///<summary> Starts the server </summary>
    public static void Start(int _maxPlayers, int _port)
    {
        maxPlayers = _maxPlayers;
        Port = _port;

        LogManager.WriteInfo($"Starting server...");
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        LogManager.WriteInfo($"Server started successfully on Port {Port}");
    }

    /// <summary>
    /// Tries to connect the client
    /// </summary>
    /// <param name="_result"></param>
    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        LogManager.WriteInfo($"Incoming connection from {client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= maxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(client);
                return;
            }
        }

        LogManager.WriteInfo($"{client.Client.RemoteEndPoint} failed to connect: Server Full");
    }

    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpListener.EndReceive(_result, ref clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (data.Length < 4) return;

            using (Packet _packet = new Packet(data))
            {
                int clientId = _packet.ReadInt();

                if (clientId == 0) return; //not checking this could cause a server crash because the ID of 0 doesn't exist

                if (clients[clientId].udp.endPoint == null) //if true, it means that this is a new connection and the packet received should be the empty one that opens up the port
                {
                    clients[clientId].udp.Connect(clientEndPoint);
                    return; //to avoid attempting to handle the data
                }

                if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())
                {
                    clients[clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            LogManager.WriteError($"Error receiving UDP data: {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint clientEndPoint, Packet _packet)
    {
        try
        {
            if (clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            LogManager.WriteError($"Error sending data to {clientEndPoint} via UDP: {_ex}");
        }
    }

    /// <summary>
    /// Allocates space determined by the maxPlayers variable for clients
    /// </summary>
    private static void InitializeServerData()
    {
        for (int i = 1; i <= maxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerPosition, ServerHandle.PlayerPosition },
                { (int)ClientPackets.currentWeapon, ServerHandle.CurrentWeapon},
                { (int)ClientPackets.cameraRotation, ServerHandle.CameraRotation},
                { (int)ClientPackets.cameraRecoil, ServerHandle.CameraRecoil},
            };

        LogManager.WriteInfo("Initialized packets");
    }

    public static void Stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }
}