using UnityEngine;

public class ServerSend
{
    /// <summary>
    /// Prepares data to be sent to the specified client via TCP
    /// /// </summary>
    /// <param name="_toClient">The client where the packet is sent</param>
    /// <param name="_packet">The packet that is sent</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>
    /// Prepares data to be sent to the specified client via UDP
    /// /// </summary>
    /// <param name="_toClient">The client where the packet is sent</param>
    /// <param name="_packet">The packet that is sent</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>
    /// A method that sends a packet to all the clients via TCP
    /// </summary>
    /// <param name="_packet"></param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    /// <summary>
    /// A method that sends a packet to all the clients except for one via TCP
    /// </summary>
    /// <param name="_packet"></param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            if (i != _exceptClient) Server.clients[i].tcp.SendData(_packet);
        }
    }

    /// <summary>
    /// A method that sends a packet to all the clients via UDP
    /// </summary>
    /// <param name="_packet"></param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    /// <summary>
    /// A method that sends a packet to all the clients except for one via UDP
    /// </summary>
    /// <param name="_packet"></param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.maxPlayers; i++)
        {
            if (i != _exceptClient) Server.clients[i].udp.SendData(_packet);
        }
    }

    /// <summary>
    /// Sends welcome message from the server to the client that has connected 
    /// </summary>
    /// <param name="_toClient"></param>
    /// <param name="_msg"></param>
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        //Sending this packet through TCP because this is an important message which we are only sending 
        //once per player that needs to be spawned so we can't really afford to lose this packet
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void PlayerPositionToAll(int _id, Vector3 _position, Quaternion _rotation)
    {
        using(Packet packet = new Packet((int)ServerPackets.playerPositionToAll))
        {
            packet.Write(_id);
            packet.Write(_position);
            packet.Write(_rotation);

            SendUDPDataToAll(_id, packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            packet.Write(_playerId);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerWeapon(int _playerId, int _weaponId)
    {
        using(Packet packet = new Packet((int)ServerPackets.playerWeapon))
        {
            packet.Write(_playerId);
            packet.Write(_weaponId);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerWeaponRotation(int _playerId, Quaternion _rotation)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerWeaponRotation))
        {
            packet.Write(_playerId);
            packet.Write(_rotation);

            SendUDPDataToAll(_playerId, packet);
        }
    }

    public static void PlayerCameraRecoil(int _playerId, Vector3 _recoilRotation)
    {
        using(Packet packet = new Packet((int)ServerPackets.playerCameraRecoil))
        {
            packet.Write(_playerId);
            packet.Write(_recoilRotation);

            SendUDPDataToAll(_playerId, packet);
        }
    }
}