using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behaviours;
using UnityEngine.AI;
using System;

public class AIPlayer : CommonBehaviours
{
    [SerializeField] private List<Vector3> rampPositions = new List<Vector3>();
    private Vector3 finalPosition;

    private bool collecting,isFalling;
    private GameObject lastFloor;

    private NavMeshAgent navMeshAgent;
    private StateMachine _stateMachine;


    protected override void Awake()
    {
        base.Awake();
        _stateMachine = new StateMachine();
        navMeshAgent = GetComponent<NavMeshAgent>();

    }
    protected override void Update()
    {
        base.Update();

        //Weird bug because navmeshagent with rigiddbody collisions sometimes send AI's flying.
        if (gameObject.transform.position.y <= -5f)
        {
            navMeshAgent.enabled = false;
            this.enabled = false;
        }

        _stateMachine.Tick();
    }

    protected override void Start()
    {
        base.Start();

        var collectBlocks = new CollectPartsState(this, animator, navMeshAgent);
        var goTowardsEnd = new GoTowardsRampState(this, animator, finalPosition);
        var idleState = new IdleState(this, animator);
        var falling = new FallingState(this, animator);

        At(collectBlocks, goTowardsEnd, EnoughBlocks(true));
        At(idleState, collectBlocks, EnoughBlocks(false));
        At(goTowardsEnd, collectBlocks, ZeroBlocks());
        At(falling, collectBlocks, Falling(false));

        _stateMachine.AddAnyTransition(falling, Falling(true));


        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

        _stateMachine.SetState(idleState);

        Func<bool> EnoughBlocks(bool value)
        {
            return delegate
            {
                var canCollect = blockStackCount < 8 && collecting;

                return value
                ? !canCollect
                : canCollect;
            };
        }

        Func<bool> ZeroBlocks() => () => blockStackCount == 0;

        Func<bool> Falling(bool value)
        {

            return delegate
            {
                var Fall = isFalling;

                return value
                    ? Fall
                    : !Fall;
            };
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.tag == "Floor" && collision.gameObject != lastFloor)
        {
            currentGrid++;
            lastFloor = collision.gameObject;
            collecting = true;
        }
        */
    }

}