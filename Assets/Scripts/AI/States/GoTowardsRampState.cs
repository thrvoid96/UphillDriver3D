using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

//OnEnter get block positions and collect them.
public class GoTowardsRampState : IState
{
    private Animator _animator;
    private AIPlayer _aIPlayer;
    private Vector3 _rampPosition;


    public GoTowardsRampState(AIPlayer aIPlayer, Animator animator, Vector3 rampPosition)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
    }

    public void OnEnter()
    {

    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }


}