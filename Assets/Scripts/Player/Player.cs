using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public string message;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
    }

    public void UpdatePosition(Vector3 _position, Quaternion _rotation)
    {
        transform.position = _position;
        transform.localRotation = _rotation;

        ServerSend.PlayerPositionToAll(id, transform.position, transform.rotation);
    }
}

