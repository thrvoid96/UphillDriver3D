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

        var list = CollectablePartSpawner.instance.getBlockPositionsForPlayer(_aIPlayer.getCurrentGrid, _aIPlayer.getPlayerNum);

        for (int i = 0; i < list.Count; i++)
        {
            destinations.Add(list[i]);
        }

        destinations = destinations.OrderBy(i => Guid.NewGuid()).ToList();

        GotoNextPoint();
    }

    public void OnExit()
    {
        _aIPlayer.DOKill();
    }

    public void Tick()
    {

    }

    private void GotoNextPoint()
    {
        RaycastHit hit;
        LayerMask partMask = LayerMask.GetMask("Part" + _aIPlayer.getPlayerNum);

        //If AI collided with blocks on the way, remove them from destinations list if they're not there anymore

        //Debug.DrawRay(destinations[0] + new Vector3(0f, 1.5f, 0f), Vector3.down * Mathf.Infinity, Color.green, 10f);

        var finalDest = destinations[0] + new Vector3(0, -0.8f, 0);

        if (Physics.Raycast(destinations[0] + new Vector3(0f, 1.5f, 0f), Vector3.down, out hit, Mathf.Infinity, partMask, QueryTriggerInteraction.Collide))
        {
            _aIPlayer.calculateValues(finalDest);

            _aIPlayer.transform.DOLookAt(finalDest, _aIPlayer.rotDuration).SetEase(Ease.InOutSine).OnUpdate(() =>
            {

                _aIPlayer.SmoothMovement();

            }).OnComplete(() =>
            {
                _aIPlayer.transform.DOLookAt(finalDest, 1f).SetEase(Ease.InOutSine);
                _aIPlayer.transform.DOMove(destinations[0] + new Vector3(0, -0.8f, 0), _aIPlayer.moveDuration).SetEase(Ease.InOutSine).OnComplete(() =>
                {
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