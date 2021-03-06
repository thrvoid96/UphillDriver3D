using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

//OnEnter get block positions and collect them.
public class IdleState : IState
{
    private Animator _animator;
    private AIPlayer _aIPlayer;
    
    public IdleState(AIPlayer aIPlayer, Animator animator)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
    }

    public void OnEnter()
    {
        _aIPlayer.StopWheels();
    }

    public void OnExit()
    {
    }

    public void Tick()
    {

    }
 

}