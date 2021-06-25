using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerController : PlayerController
{
    public Transform cameraOffSetTransform;

    public int id;
    public string username;
    public int maxHealth = 100;
    public int health;
    public int armor;
    public int kills;
    private bool[] inputs;

    void Start()
    {
        isServer = true;

        base.Start();

        timeStep = Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;

        isServer = true;

        health = maxHealth;
        armor = 0;
        kills = 0;

        inputs = new bool[5];
    }

    public void Update()
    {
        timeStep = Time.fixedDeltaTime;
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        if (health <= 0)
        {
            return;
        }

        x = 0.0f;
        z = 0.0f;
        jumpQueued = false;

        if (inputs[0])
        {
            z = 1.0f;
        }
        if (inputs[1])
        {
            z = -1.0f;
        }
        if (inputs[2])
        {
            x = -1.0f;
        }
        if (inputs[3])
        {
            x = 1.0f;
        }

        jumpQueued = inputs[4];

        //base.Update();

        ServerSend.PlayerPosition(this, inputs);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation, Vector3 position)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
        transform.position = position;
    }

    public void Shoot(Vector3 _viewDirection)
    {
        float distance = 100.0f;

        //if (Physics.Raycast(cameraOffSetTransform.position, _viewDirection, out RaycastHit _hit, 25f))
        if (Physics.Raycast(cameraOffSetTransform.position, _viewDirection, out RaycastHit _hit))
        {
            distance = _hit.distance;
            if (_hit.collider.CompareTag("Player"))
            {
                bool scoredKill = _hit.collider.GetComponent<ServerPlayerController>().TakeDamage(100);

                if (scoredKill)
                {
                    kills++;
                    ServerSend.GameKill(this);

                    if (kills >= Server.MaxKills)
                    {
                        ServerSend.GameRestart();
                    }
                    else
                    {
                        _hit.collider.GetComponent<ServerPlayerController>().startRespawn();
                    }
                }
            }
        }

        ServerSend.PlayerShoot(this, cameraOffSetTransform.position, _viewDirection, distance);
    }

    public bool TakeDamage(int _damage)
    {
        if (health <= 0f)
        {
            return false;
        }

        bool died = false;

        health -= _damage;
        if (health <= 0)
        {
            died = true;
            health = 0;
            characterController.enabled = false;
            //transform.position = new Vector3(0f, 25f, 0f);
            //ServerSend.PlayerPosition(this);
            //StartCoroutine(Respawn(5.0f));
        }

        ServerSend.PlayerHealth(this);

        return died;
    }

    public void startRespawn()
    {
        StartCoroutine(Respawn(5.0f));
    }

    public IEnumerator Respawn(float time)
    {
        yield return new WaitForSeconds(time);

        health = maxHealth;
        armor = 0;
        characterController.enabled = true;

        Vector3 randomSpawnPosition = NetworkManager.instance.getRandomSpawnPosition();

        ServerSend.PlayerRespawned(this, randomSpawnPosition);
    }
}