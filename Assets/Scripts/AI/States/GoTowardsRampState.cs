using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

//OnEnter get block positions and collect them.
public class GoTowardsRampState : IState
{
    private Animator _animator;
    private AIPlayer _aIPlayer;

    private Vector3 destination;

    public GoTowardsRampState(AIPlayer aIPlayer, Animator animator)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
    }

    public void OnEnter()
    {
        var randomIndex = UnityEngine.Random.Range(0, SceneSetup.instance.floorsOnScene[_aIPlayer.getCurrentGrid].AIPositionsToGo.Count);

        destination = SceneSetup.instance.floorsOnScene[_aIPlayer.getCurrentGrid].AIPositionsToGo[randomIndex].position;

        float distance = Vector3.Distance(_aIPlayer.transform.position, destination);
        float clampTime = Mathf.Clamp(distance / 20f, 2f, 4f);

        _aIPlayer.transform.DOMove(new Vector3(destination.x,_aIPlayer.transform.position.y, destination.z),clampTime).SetEase(Ease.InOutSine);
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }


}