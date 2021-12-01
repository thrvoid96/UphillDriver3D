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
    private float a = 100f;

    public OnRampState(AIPlayer aIPlayer, Animator animator)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
    }

    public void OnEnter()
    {
        addAmount = Random.Range(1, LevelHolder.instance.howManyFloors[_aIPlayer.getCurrentGrid].blocksToPassRamp);
        _aIPlayer.CalculateValues(_aIPlayer.finalPos);
        a = 100f;

        _aIPlayer.transform.DOMove(_aIPlayer.finalPos, _aIPlayer.moveDuration * _aIPlayer.speedRatio).SetEase(Ease.InOutSine).OnStart(() => {

            _aIPlayer.StartTrails();
            
            DOTween.To(() => a, x => a = x, 0f, _aIPlayer.moveDuration * _aIPlayer.speedRatio * 0.5f)
                .OnComplete(() =>
                {
                    _aIPlayer.StopTrails();
                });
            

        }).OnComplete(()=> {
            
                if (_aIPlayer.coefficient < 0.9f)
                {
                    a = 100f;
                    
                    DOTween.To(() => a, x => a = x, 0f, 1.5f)
                        .OnStart(() =>
                        {
                            _aIPlayer.StartTrails();
                            
                            _aIPlayer.StartSmokes();
                        }).OnComplete(() =>
                        {
                            _aIPlayer.GetDownFromRamp();
                            
                            _aIPlayer.StopSmokes();

                            a = 100f;
                            DOTween.To(() => a, x => a = x, 0f, _aIPlayer.moveDuration * _aIPlayer.speedRatio * 0.5f)
                                .OnComplete(() =>
                                {
                                    _aIPlayer.StopTrails();
                                });
                        });
                }
                else
                {
                    _aIPlayer.StopTrails();
                    
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