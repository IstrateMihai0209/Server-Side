using System;
using UnityEngine;

public class ServerHandle
{
    /// <summary>
    /// Welcome received from the client 
    /// </summary>
    /// <param name="_fromClient">ID of the client</param>
    /// <param name="_packet">The packet sent</param>
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        //the same order we read them in the ClientSend script
        int clientIdCheck = _packet.ReadInt();
        string username = _packet.ReadString();

        LogManager.WriteInfo($"{username} ({Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint}) connected succesfully and is now player {_fromClient}.");

        if (_fromClient != clientIdCheck)
        {
            LogManager.WriteInfo($"Player \"{username}\" (ID: {_fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
        }

        Server.clients[_fromClient].SendIntoGame(username);
    }

    public static void PlayerPosition(int _fromClient, Packet _packet)
    {
        Vector3 position = _packet.ReadVector3();
        Quaternion rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.UpdatePosition(position, rotation);
    }
}