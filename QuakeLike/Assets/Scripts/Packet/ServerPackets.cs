using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServerPackets
{
    welcome = 1,
    spawnPlayer,
    playerPosition,
    playerRotation,
    playerDisconnected,
    playerHealth,
    playerRespawned,
    playerShoot,
    gameKill,
    gameRestart
}
