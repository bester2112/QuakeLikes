using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGameInstance : MonoBehaviour
{
    public static ClientGameInstance instance;

    public static Dictionary<int, ConnectedPlayerData> players = new Dictionary<int, ConnectedPlayerData>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject _player;
        if (id == ConnectionToServer.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, position, rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, position, rotation);
        }

        _player.GetComponent<ConnectedPlayerData>().Initialize(id, username);
        players.Add(id, _player.GetComponent<ConnectedPlayerData>());
    }
}