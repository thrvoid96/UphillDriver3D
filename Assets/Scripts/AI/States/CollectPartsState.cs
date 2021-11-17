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
        destinations.Clear();

        var randomIndex = UnityEngine.Random.Range(SceneSetup.instance.floorsOnScene[_aIPlayer.currentFloor].gridIndexes.First(), SceneSetup.instance.floorsOnScene[_aIPlayer.currentFloor].gridIndexes.Last() + 1);

        _aIPlayer.currentGrid = randomIndex;
        
        var list = CollectablePartSpawner.instance.getBlockPositionsForPlayer(randomIndex, _aIPlayer.getPlayerNum);

        for (int i = 0; i < list.Count; i++)
        {
            destinations.Add(list[i]);
        }

        destinations = destinations.OrderBy(i => Guid.NewGuid()).ToList();

        GotoNextPoint();
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }

    private void GotoNextPoint()
    {
        _aIPlayer.lastTweenIsComplete = false;

        RaycastHit hit;
        LayerMask partMask = LayerMask.GetMask("Part" + _aIPlayer.getPlayerNum);

        //If AI collided with blocks on the way, remove them from destinations list if they're not there anymore

        //Debug.DrawRay(destinations[0] + new Vector3(0f, 1.5f, 0f), Vector3.down * Mathf.Infinity, Color.green, 10f);

        if (Physics.Raycast(destinations[0] + new Vector3(0f, 1.5f, 0f), Vector3.down, out hit, Mathf.Infinity, partMask, QueryTriggerInteraction.Collide))
        {
            float distance = Vector3.Distance(_aIPlayer.transform.position, destinations[0]);
            float clampTime = Mathf.Clamp(distance / 20f, 1f ,8f);
            _aIPlayer.transform.DOMove(destinations[0], clampTime).SetEase(Ease.InOutSine).OnComplete(() => {

                if (!_aIPlayer.collectAmountReached)
                {
                    destinations.RemoveAt(0);
                    GotoNextPoint();
                }
                else
                {
                    _aIPlayer.lastTweenIsComplete = true;
                }
            });
        }
        else
        {
            if (!_aIPlayer.collectAmountReached)
            {
                destinations.RemoveAt(0);
                GotoNextPoint();
            }
            else
            {
                _aIPlayer.lastTweenIsComplete = true;
            }
        }


    }

}