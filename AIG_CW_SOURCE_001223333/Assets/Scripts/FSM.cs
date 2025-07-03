using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void ExecuteStateLogic(StateMachine sm);
    public abstract void Transition(StateMachine sm);
}
  
public class Idle : State
{
    public override void ExecuteStateLogic(StateMachine sm)
    {
        //logic idle state //does nothing
        //sm.CreateGrid();

    }

    public override void Transition(StateMachine sm)
    {
        //logic for transitions
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //sm.isChangedState = true;
            sm.currentState = new GameOfLife();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //sm.isChangedState = true;
            sm.currentState = new DayAndNight();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //sm.isChangedState = true;
            sm.currentState = new Seeds();
        }
    }
}

public class GameOfLife : State
{

    public override void ExecuteStateLogic(StateMachine sm)
    {
        //sm.isChangedState = false;
        sm.RandomiseGridCells();
        sm.ConwaysGame();
    }

    public override void Transition(StateMachine sm)
    {
        //logic for transitions
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //sm.isChangedState = true;
            sm.currentState = new DayAndNight();

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //sm.isChangedState = true;
            sm.currentState = new Seeds();

        }
    }
}


public class DayAndNight : State
{
    public override void ExecuteStateLogic(StateMachine sm)
    {
        //sm.isChangedState = false;
        sm.RandomiseGridCells();
        sm.DayNight();
        
    }

    public override void Transition(StateMachine sm)
    {
        //logic for transitions
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //sm.isChangedState = true;
            sm.currentState = new GameOfLife();

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //sm.isChangedState = true;
            sm.currentState = new Seeds();

        }
    }
}

public class Seeds : State
{
    public override void ExecuteStateLogic(StateMachine sm)
    {
        //sm.isChangedState = false;
        sm.RandomiseGridCells();
        sm.Seed(); 
        
    }


    public override void Transition(StateMachine sm)
    {
        //logic for transitions
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //sm.isChangedState = true;
            sm.currentState = new GameOfLife();

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //sm.isChangedState = true;
            sm.currentState = new DayAndNight();

        }
    }
}