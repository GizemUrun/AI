using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class State
{
    public abstract void ExecuteStateLogic(NPCStateMachine npc);
    public abstract void Transition(NPCStateMachine npc);
}

public class Idle : State
{

    public override void ExecuteStateLogic(NPCStateMachine npc)
    {
        //logic idle state //does nothing
    }


    public override void Transition(NPCStateMachine npc)
    {
        //logic for transitions
        if (Input.GetKeyDown(KeyCode.I))
        {
            npc.currentState = new Walk();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            npc.currentState = new Jumping();
        }

    }
}

public class Walk: State
{
    public override void ExecuteStateLogic(NPCStateMachine npc)
    {
        Move(npc);
    }

    public void Move(NPCStateMachine npc)
    {
        float move = npc.walkSpeed;
        npc.rb.velocity = new Vector3(npc.rb.velocity.x, npc.rb.velocity.y, move);
    }

    /*public void Move(NPCStateMachine npc)
    {
        float move = npc.movingRight ? npc.walkSpeed: -npc.walkSpeed;
        npc.rb.velocity = new Vector2(move,npc.rb.velocity.y);
    }

    public void FlipAtEdges(NPCStateMachine npc)
    {
        bool hitEdgeRight =
            !Physcics2D.OverlapCircle(npc.edgeCheckPointRight.position, 0.1f, LayerMask.GetMask("Platforms"));
        bool hitEdgeLeft =
            !Physcics2D.OverlapCircle(npc.edgeCheckPointLeft.position, 0.1f, LayerMask.GetMask("Platforms"));

        if (npc.movingRight && hitEdgeRight)
        {
            npc.movingRight = false;
            npc.spriteRenderer.flipX = true;
        }
        else if(!npc.movingRight && hitEdgeLeft)
        {
            npc.movingRight = true;
            npc.spriteRenderer.flipX = false;
        }

    } */

    public override void Transition(NPCStateMachine npc)
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            npc.currentState = new Idle();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            npc.currentState = new Jumping();
        }
    }
}

public class Jumping : State
{
    public override void ExecuteStateLogic(NPCStateMachine npc)
    {
            Jump(npc);
    }

    public void Jump(NPCStateMachine npc)
    {
        //jump logic
        float jump = npc.jumpForce;
        float wait = npc.jumpCooldown;
        float nextTime = npc.nextJumpTime;

        if (wait <= nextTime)
        {
            npc.rb.velocity = new Vector3(npc.rb.velocity.x, jump, npc.rb.velocity.z); //doesn't come back down because ITS VELOCITY //CHANGE THIS
            npc.jumpCooldown = 1f;
            
            //npc.rb.velocity.y -= jump * Time.deltaTime;
        }
        


    }
    

    public override void Transition(NPCStateMachine npc)
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            npc.currentState = new Idle();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            npc.currentState = new Walk();
        }
    }
}