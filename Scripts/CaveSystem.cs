using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveSystem : MonoBehaviour
{
    public float spawnTime = 2;
    public GameObject enemyPrefab;
    public GameObject spawnPoint;
    float timeLeft;


    void Update()
    {
        timeLeft -= Time.deltaTime;
        if(timeLeft < 0)
        {
            Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            timeLeft = spawnTime;
        }
    }
}
