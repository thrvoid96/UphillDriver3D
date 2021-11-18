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

    private float smoothSpeed;

    public GoTowardsRampState(AIPlayer aIPlayer, Animator animator)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
    }

    public void OnEnter()
    {
        _aIPlayer.ResetSmoothValue();

        var randomIndex = UnityEngine.Random.Range(0, SceneSetup.instance.floorsOnScene[_aIPlayer.getCurrentGrid].AIPositionsToGo.Count);

        var destination = SceneSetup.instance.floorsOnScene[_aIPlayer.getCurrentGrid].AIPositionsToGo[randomIndex].position;

        var finalDest = new Vector3(destination.x, _aIPlayer.transform.position.y, destination.z);

        _aIPlayer.calculateValues(finalDest);

        _aIPlayer.transform.DOLookAt(finalDest, _aIPlayer.rotDuration).SetEase(Ease.InOutSine).OnUpdate(() =>
        {
            _aIPlayer.SmoothMovement();

        }).OnComplete(() => 
        {
            _aIPlayer.transform.DOLookAt(finalDest, 1f).SetEase(Ease.InOutSine);
            _aIPlayer.transform.DOMove(finalDest, _aIPlayer.moveDuration).SetEase(Ease.InOutSine);
        });
    }

    public void OnExit()
    {
        _aIPlayer.DOKill();
    }

    public void Tick()
    {

    }

}