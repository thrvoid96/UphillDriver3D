using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

//OnEnter get block positions and collect them.
public class OnRampState : IState
{
    private Animator _animator;
    private AIPlayer _aIPlayer;

    private float smoothInput;
    private int amountToAdd;
    private bool isGoingUp;


    public OnRampState(AIPlayer aIPlayer, Animator animator, int increaseAmount)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
        amountToAdd = increaseAmount;
    }

    public void OnEnter()
    {
        isGoingUp = true;
        _aIPlayer.getCurrentSpeed = _aIPlayer.getMaxSpeed;
        smoothInput = 0f;
    }

    public void OnExit()
    {       
        _aIPlayer.collectBlockAmount += amountToAdd;
        _aIPlayer.DOKill();
    }

    public void Tick()
    {
        if (isGoingUp)
        {
            smoothInput += Time.deltaTime;
        }
        else
        {
            smoothInput -= Time.deltaTime;
        }

        smoothInput = Mathf.Clamp(smoothInput, -1f, 1f);
        

        var clampValue = Mathf.Clamp(2f + (0.02f *  _aIPlayer.getCollectedAmount), 0.1f, 2f);
        _aIPlayer.getCurrentSpeed = Mathf.Clamp(_aIPlayer.getCurrentSpeed - (clampValue / _aIPlayer.rampAngleX * smoothInput * _aIPlayer.rampClimbSmoothValue), 0, _aIPlayer.getMaxSpeed);
        _aIPlayer.transform.Translate(_aIPlayer.getCurrentSpeed * Time.deltaTime * smoothInput * Vector3.forward, Space.Self);

        if(_aIPlayer.getCurrentSpeed == 0f)
        {
            isGoingUp = false;
        }
    }

}