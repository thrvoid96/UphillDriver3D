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

       var amount= _aIPlayer.transform.DOKill();

       goTowardsPosition();
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        
    }


    private void goTowardsPosition()
    {
        var randomIndex = UnityEngine.Random.Range(0, LevelHolder.instance.howManyFloors[_aIPlayer.getCurrentGrid].AIPositionsToGo.Count);

        var destination = LevelHolder.instance.howManyFloors[_aIPlayer.getCurrentGrid].AIPositionsToGo[randomIndex];

        var finalDest = new Vector3(destination.x, _aIPlayer.transform.position.y, destination.z);

        _aIPlayer.CalculateValues(finalDest);
        
        _aIPlayer.RotateWheelsRespectively(_aIPlayer.rotDuration * _aIPlayer.speedRatio);

        _aIPlayer.transform.DOLookAt(finalDest, _aIPlayer.rotDuration * _aIPlayer.speedRatio).SetEase(Ease.InOutSine).OnUpdate(() =>
        {
            _aIPlayer.SmoothMovement();

        }).OnComplete(() => 
        {
            _aIPlayer.CenterWheels(0.25f);
            _aIPlayer.transform.DOLookAt(finalDest, 1f * _aIPlayer.speedRatio).SetEase(Ease.InOutSine);
            _aIPlayer.transform.DOMove(finalDest, _aIPlayer.moveDuration * _aIPlayer.speedRatio).SetEase(Ease.InOutSine);
        });
    }
}