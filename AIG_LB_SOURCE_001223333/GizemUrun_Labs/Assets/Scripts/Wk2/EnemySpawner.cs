using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject Enemies;
    int spawnRate = 5;
    [SerializeField] GameDirector director;
    int playerHealthCount;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Spawn()
    {
        
        if (spawnRate == 0)
        {
           
            SpawnRateChanged();
            Instantiate(Enemies);
            Enemies.GetComponent<Damage>().director = director;
        }
    }

    void SpawnRateChanged()
    {
        playerHealthCount = director.currentHealth;

        if(director != null)
        {
            if(playerHealthCount <= 20 && playerHealthCount >= 16) //if health is 16-20
            {
                spawnRate = 250;
            }

            if (playerHealthCount < 16 && playerHealthCount > 10) //if health is 11 - 15
            {
                spawnRate = 500;
            }
            if(playerHealthCount <= 10 && playerHealthCount >= 6) //health = 6-10
            {
                spawnRate = 750;
            }
            if (playerHealthCount < 5) //health is less than 5
            {
                spawnRate = 1000;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        spawnRate--;
        Spawn();
    }
}
