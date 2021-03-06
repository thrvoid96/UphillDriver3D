using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using UnityEngine;

//OnEnter get block positions and collect them.
public class CollectPartsState : IState
{
    private Animator _animator;
    private AIPlayer _aIPlayer;

    private List<Vector3> destinations = new List<Vector3>();

    public CollectPartsState(AIPlayer aIPlayer, Animator animator)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
    }

    public void OnEnter()
    {
        _aIPlayer.ResetSmoothValue();

        destinations.Clear();

        randomizePoints();

        GotoNextPoint();
        
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        
    }

    private void randomizePoints()
    {
        var list = LevelHolder.instance.getBlockPositionsForPlayer(_aIPlayer.getCurrentGrid, _aIPlayer.getPlayerNum);

        for (int i = 0; i < list.Count; i++)
        {
            destinations.Add(list[i]);
        }

        destinations = destinations.OrderBy(i => Guid.NewGuid()).ToList();
    }

    private void GotoNextPoint()
    {
        var partMask = LayerMask.GetMask("Part");

        //If AI collided with blocks on the way, remove them from destinations list if they're not there anymore

        //Debug.DrawRay(destinations[0] + new Vector3(0f, 1.5f, 0f), Vector3.down * Mathf.Infinity, Color.green, 10f);
        if (destinations.Count == 0)
        {
            randomizePoints();
            GotoNextPoint();
            return;
        }
        var finalDest = destinations[0] + new Vector3(0, -0.5f, 0);

        if (Physics.Raycast(finalDest + new Vector3(0f, 4f, 0f), Vector3.down, 2f, partMask, QueryTriggerInteraction.Collide))
        {
            _aIPlayer.CalculateValues(finalDest);
            
            _aIPlayer.RotateWheelsRespectively(_aIPlayer.rotDuration * _aIPlayer.speedRatio);

            _aIPlayer.transform.DOLookAt(finalDest, _aIPlayer.rotDuration * _aIPlayer.speedRatio).SetEase(Ease.InOutSine).OnUpdate(() =>
            {
                _aIPlayer.SmoothMovement();

            }).OnComplete(() =>
            {
                _aIPlayer.CenterWheels(0.25f);
                
                _aIPlayer.transform.DOLookAt(finalDest, 1f * _aIPlayer.speedRatio).SetEase(Ease.InOutSine);
                _aIPlayer.transform
                    .DOMove(finalDest, _aIPlayer.moveDuration * _aIPlayer.speedRatio)
                    .SetEase(Ease.InOutSine)
                    .OnUpdate(() =>
                    {
                        _aIPlayer.TurnWheelsForward();

                    }).OnComplete(() =>
                    {
                        _aIPlayer.ResetSmoothValue();
                        _aIPlayer.StopWheels();
                        Loop();
                    }); 
            });
        }
        else
        {
            Loop();
        }


    }

    private void Loop()
    {
        if (!_aIPlayer.collectAmountReached && LevelManager.gameState == GameState.Normal)
        {
            destinations.RemoveAt(0);
            GotoNextPoint();
        }
    }

}