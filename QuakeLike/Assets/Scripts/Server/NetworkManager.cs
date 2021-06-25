using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;

    public Transform[] spawnPositions;

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

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(16, 25, 26950);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public ServerPlayerController InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<ServerPlayerController>();
    }

    public Vector3 getRandomSpawnPosition()
    {
        int index = Random.Range(0, spawnPositions.Length);

        return spawnPositions[index].position;
    }
}