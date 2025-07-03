using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCStateMachine : MonoBehaviour
{
    public State currentState;
    public Rigidbody rb;
    public float walkSpeed = 2.0f;
    public float jumpForce = 5.0f;
    public float jumpCooldown = 1f;
    public float nextJumpTime;

    

    void Start()
    {
        currentState = new Idle();
        nextJumpTime = 0f;
    }

    void Update()
    {
        currentState.ExecuteStateLogic(this);
        currentState.Transition(this);
        if(jumpCooldown > nextJumpTime)
        {
            jumpCooldown--;
        }
        

        
    }
}
