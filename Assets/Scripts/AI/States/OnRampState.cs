using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

//OnEnter get block positions and collect them.
public class OnRampState : IState
{
    private Animator _animator;
    private AIPlayer _aIPlayer;

    private int addAmount;


    public OnRampState(AIPlayer aIPlayer, Animator animator)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
    }

    public void OnEnter()
    {
        addAmount = Random.Range(1, LevelHolder.instance.howManyFloors[_aIPlayer.getCurrentGrid].blocksToPassRamp);
        var distance = Vector3.Distance(_aIPlayer.rampStartPos, _aIPlayer.finalPos);
        var moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);

        _aIPlayer.transform.DOMove(_aIPlayer.finalPos, moveDuration * _aIPlayer.speedRatio).SetEase(Ease.InOutSine).OnStart(() => {

            _aIPlayer.StartTrails();

        }).OnComplete(()=> {

                if (_aIPlayer.coefficient < 0.9f)
                {
                    _aIPlayer.GetDownFromRamp();
                    
                    _aIPlayer.StartTrails();
                }
                else
                {
                    _aIPlayer.ExitRampComplete();
                }
        });
    }

    public void OnExit()
    {
        _aIPlayer.collectBlockAmount = _aIPlayer.getCollectedPartCount + addAmount;
    }

    public void Tick()
    {
        
    }

}