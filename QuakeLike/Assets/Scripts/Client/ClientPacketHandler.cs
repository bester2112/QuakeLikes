using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientPacketHandler : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();

        Debug.Log($"Message from server: {msg}");
        ConnectionToServer.instance.myId = myId;
        ClientPacketSender.WelcomeReceived();

        ConnectionToServer.instance.udp.Connect(((IPEndPoint)ConnectionToServer.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        Debug.Log("Player spawned" + ClientGameInstance.players[id].username);

        ClientGameInstance.instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();

        if (id != ConnectionToServer.instance.myId || !LocalPlayerController.alive)
        {
            Debug.Log("Player moved" + ClientGameInstance.players[id].username);

            Vector3 position = packet.ReadVector3();

            //ClientGameInstance.players[id].transform.position = position;
            ClientGameInstance.players[id].oldPosition = ClientGameInstance.players[id].newPosition;
            ClientGameInstance.players[id].newPosition = position;
            ClientGameInstance.players[id].lerpTime = 0.0f;

            for (int i = 0; i < ClientGameInstance.players[id].inputs.Length; i++)
            {
                ClientGameInstance.players[id].inputs[i] = packet.ReadBool();
            }
        }

    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();

        if (id != ConnectionToServer.instance.myId || !LocalPlayerController.alive)
        {
            Quaternion rotation = packet.ReadQuaternion();

            ClientGameInstance.players[id].transform.rotation = rotation;
            //ClientGameInstance.players[id].oldTransform.rotation = ClientGameInstance.players[id].newTransform.rotation;
            //ClientGameInstance.players[id].newTransform.rotation = rotation;
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(ClientGameInstance.players[_id].gameObject);
        ClientGameInstance.players.Remove(_id);
    }

    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _health = _packet.ReadInt();

        ClientGameInstance.players[_id].SetHealth(_health);
    }

    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 position = _packet.ReadVector3();

        ClientGameInstance.players[_id].transform.position = position;

        ClientGameInstance.players[_id].Respawn();
    }

    public static void PlayerShoot(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Vector3 forward = packet.ReadVector3();
        float distance = packet.ReadFloat();

        ClientGameInstance.players[id].Shoot(position, forward, distance);
    }

    public static void GameKill(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int kills = _packet.ReadInt();

        ClientGameInstance.players[_id].kills = kills;
    }

    public static void GameRestart(Packet _packet)
    {
        foreach (ConnectedPlayerData cpd in ClientGameInstance.players.Values)
        {
            if (cpd != null)
            {
                cpd.Die();
                ClientGameInstance.instance.StartCoroutine(cpd.ResetPlayer(8.0f));
            }
        }
    }
}