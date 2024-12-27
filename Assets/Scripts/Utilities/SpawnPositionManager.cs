using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionManager : MonoBehaviour
{
    [Header("Player Spawn Positions")]
    public List<GameObject> spawnPoints = new List<GameObject>();

    public int spawnIndex = 0;
}
