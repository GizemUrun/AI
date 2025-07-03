using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
 // GameDirector director;
    
    public GameDirector director;

    public Transform player;
    public int maxRange;
    public int minRange;
    private Vector3 toTarget;
    float move = 2f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }
    
    //MOVE TO PLAYER SCRIPT
    void FindPlayer()
    {
        //if(player.position )
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") player = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") player = null;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        { 
            director.PlayerLosesHealth();
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }
        
        if (player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, move * Time.deltaTime);
        }
        
        
    }
}
