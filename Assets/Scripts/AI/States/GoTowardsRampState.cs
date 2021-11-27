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

    private float timer;
    private Vector3 lastPos;

    public GoTowardsRampState(AIPlayer aIPlayer, Animator animator)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
    }

    public void OnEnter()
    {
        timer = 0f;
        lastPos = _aIPlayer.transform.position;
        _aIPlayer.ResetSmoothValue();

        goTowardsPosition();
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        if (lastPos == _aIPlayer.transform.position)
        {
            timer += Time.deltaTime;
            if (timer > 0.2f)
            {
                goTowardsPosition();
                timer = 0f;
            }
        }
        else
        {
            lastPos = _aIPlayer.transform.position;
        }
    }


    private void goTowardsPosition()
    {
        var randomIndex = UnityEngine.Random.Range(0, LevelHolder.instance.howManyFloors[_aIPlayer.getCurrentGrid].AIPositionsToGo.Count);

        var destination = LevelHolder.instance.howManyFloors[_aIPlayer.getCurrentGrid].AIPositionsToGo[randomIndex];

        var finalDest = new Vector3(destination.x, _aIPlayer.transform.position.y, destination.z);

        _aIPlayer.CalculateValues(finalDest);

        _aIPlayer.transform.DOLookAt(finalDest, _aIPlayer.rotDuration).SetEase(Ease.InOutSine).OnUpdate(() =>
        {
            _aIPlayer.SmoothMovement();

        }).OnComplete(() => 
        {
            _aIPlayer.transform.DOLookAt(finalDest, 1f).SetEase(Ease.InOutSine);
            _aIPlayer.transform.DOMove(finalDest, _aIPlayer.moveDuration).SetEase(Ease.InOutSine);
        });
    }
}