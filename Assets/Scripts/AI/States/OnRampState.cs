﻿using System;
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

    private int nextCollectAmount;


    public OnRampState(AIPlayer aIPlayer, Animator animator)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
    }

    public void OnEnter()
    {
        nextCollectAmount = Random.Range(_aIPlayer.getCollectedPartCount + 1, _aIPlayer.getCollectedPartCount + LevelHolder.instance.rampsOnScene[_aIPlayer.getCurrentGrid].getBlocksNeededToClimb + 1);
        var distance = Vector3.Distance(_aIPlayer.rampStartPos, _aIPlayer.finalPos);
        var moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);

        _aIPlayer.transform.DOMove(_aIPlayer.finalPos, moveDuration).SetEase(Ease.InOutSine).OnStart(() => {

            _aIPlayer.StartTrails();

        }).OnComplete(()=> {

                if (_aIPlayer.coefficient < 0.9f)
                {
                    distance = Vector3.Distance(_aIPlayer.rampStartPos, _aIPlayer.finalPos);
                    moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);

                    _aIPlayer.transform.DOMove(_aIPlayer.rampStartPos + new Vector3(0, -1f, -10f), moveDuration).SetEase(Ease.InOutSine);

                    _aIPlayer.StartTrails();
                }                   
        });
    }

    public void OnExit()
    {       
        _aIPlayer.collectBlockAmount = nextCollectAmount;
        _aIPlayer.DOKill();
    }

    public void Tick()
    {
        
    }

}