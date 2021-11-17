using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behaviours;
using UnityEngine.AI;
using System;

public class AIPlayer : CommonBehaviours
{
    [SerializeField] private List<Vector3> rampPositions = new List<Vector3>();
    [SerializeField] private int collectBlockAmount;
    [SerializeField] private string currentState;

    public int currentGrid;
    public int currentFloor;

    public bool isCollecting;

    private Vector3 finalPosition;

    private StateMachine _stateMachine;


    protected override void Awake()
    {
        base.Awake();
        
        _stateMachine = new StateMachine();

    }
    protected override void Update()
    {
        base.Update();
        _stateMachine.Tick();
        currentState = _stateMachine.CurrentState.ToString();
    }

    protected override void Start()
    {
        base.Start();

        var collectBlocks = new CollectPartsState(this, animator);
        var goTowardsEnd = new GoTowardsRampState(this, animator, finalPosition);
        var idleState = new IdleState(this, animator);

        At(collectBlocks, goTowardsEnd, EnoughBlocks(true));
        At(idleState, collectBlocks, EnoughBlocks(false));
        At(goTowardsEnd, collectBlocks, ZeroBlocks());

        _stateMachine.AddAnyTransition(idleState, GameIsPlaying(false));

        _stateMachine.SetState(idleState);

        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

        

        Func<bool> EnoughBlocks(bool value)
        {
            return delegate
            {
                isCollecting = carPartCollector.collectedPartsCount < collectBlockAmount;

                return value
                ? !isCollecting
                : isCollecting;
            };
        }

        Func<bool> ZeroBlocks() => () => carPartCollector.collectedPartsCount == 0;

        Func<bool> GameIsPlaying(bool value) 
        {
            return delegate
            {
                var canMove = LevelManager.gameState == GameState.Normal;

                return value
                ? canMove
                : !canMove;
            };
        }

    }

}