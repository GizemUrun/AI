using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    int playerDeaths = 0;
    //int playerDetection;
    int playerHealth;
    int damageTaken = 1;
    public int currentHealth;

    public Transform spawnPoint;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        if(spawnPoint == null)
        {
            spawnPoint = GameObject.FindWithTag("Spawn").transform;
        }

        playerHealth = 20;
        currentHealth = playerHealth;
    }

    public void PlayerLosesHealth()
    {
        currentHealth -= damageTaken;
        PlayerDies();
    }

    public void PlayerDies()
    {
        if (currentHealth <= 0)
        {
            player.transform.position = spawnPoint.position;
            playerDeaths += 1;
            currentHealth = playerHealth;

        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }


}
