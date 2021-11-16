using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

//OnEnter get block positions and collect them.
public class CollectPartsState : IState
{
    private Animator _animator;
    private AIPlayer _aIPlayer;
    private NavMeshAgent _navMeshAgent;

    private RaycastHit hit;
    private LayerMask blockMask;

    private List<Vector3> destinations = new List<Vector3>();

    public CollectPartsState(AIPlayer aIPlayer, Animator animator, NavMeshAgent navMeshAgent)
    {
        _animator = animator;
        _aIPlayer = aIPlayer;
        _navMeshAgent = navMeshAgent;
    }

    public void OnEnter()
    {
        _animator.SetFloat("vertical", 1f);
        _animator.SetFloat("idleTime", 0f);
        blockMask = LayerMask.GetMask("Block");
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
        destinations.Clear();
    }

    public void Tick()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 0.1f)
        {
            GotoNextPoint();
        }

    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (destinations.Count == 0)
        {
            return;
        }

        //If AI collided with blocks on the way, remove them from destinations list if they're not there anymore

        //Debug.DrawRay(destinations[0] + new Vector3(0f, 0.5f, 0f), Vector3.down * 1f, Color.green, 10f);
        if (!Physics.Raycast(destinations[0] + new Vector3(0f, 1f, 0f), Vector3.down, out hit, 1f, blockMask))
        {
            destinations.RemoveAt(0);
            return;
        }

        // Set the agent to go to the currently selected destination
        try
        {
            _navMeshAgent.destination = destinations[0];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _aIPlayer.enabled = false;
        }

        //Debug.LogError(destinations[0]);

        // Remove reached destination
        destinations.RemoveAt(0);

    }

}