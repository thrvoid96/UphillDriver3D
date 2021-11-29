using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behaviours;
using UnityEngine.AI;
using System;
using System.Timers;
using DG.Tweening;
using Random = UnityEngine.Random;

public class AIPlayer : CommonBehaviours
{
    
    [Header("For Debug Only")]
    [SerializeField] private string currentState;
    public int collectBlockAmount;
    
    [NonSerialized] public float rotDuration, moveDuration;
    
    public bool collectAmountReached;

    private bool forceIntoIdle;
      
    private float angle, distance;
    
    public int getCollectedPartCount => carPartCollector.collectedPartsCount;
    
    private StateMachine _stateMachine;
    


    protected override void Awake()
    {
        base.Awake();
        collectBlockAmount = Random.Range(1, LevelHolder.instance.howManyFloors[getCurrentGrid].blocksToPassRamp + 1);
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
        var goTowardsRamp = new GoTowardsRampState(this, animator);
        var idle = new IdleState(this, animator);
        var onRamp = new OnRampState(this, animator);
        var collided = new CollisionState(this, animator);

        At(collectBlocks, goTowardsRamp, EnoughBlocks(true));
        
        At(idle, goTowardsRamp, EnoughBlocks(true));
        At(idle, onRamp, ClimbingRamp(true));
        
        At(idle, collectBlocks, EnoughBlocks(false));
        At(collided, collectBlocks, EnoughBlocks(false));
        At(collided, collectBlocks, Collided(false));
        At(goTowardsRamp, collectBlocks, ZeroBlocks());

        _stateMachine.AddAnyTransition(idle, CanMove(false));
        _stateMachine.AddAnyTransition(collided, Collided(true));

        _stateMachine.SetState(idle);

        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);      

        Func<bool> EnoughBlocks(bool value)
        {
            return delegate
            {
                collectAmountReached = carPartCollector.collectedPartsCount >= collectBlockAmount && !isOnRamp;

                return value
                ? collectAmountReached
                : !collectAmountReached;
            };
        }

        Func<bool> ZeroBlocks() => () => carPartCollector.collectedPartsCount == 0;


        Func<bool> ClimbingRamp(bool value)
        {
            return delegate
            {
                var move = canMove && isOnRamp;

                return value
                ? move
                : !move;
            };
        }
        
        Func<bool> Collided(bool value)
        {
            return delegate
            {
                var collide = isCollided && !isOnRamp;

                return value
                    ? collide
                    : !collide;
            };
        }
        
        Func<bool> CanMove(bool value)
        {
            return delegate
            {
                var move = canMove && LevelManager.gameState == GameState.Normal;

                return value
                ? move
                : !move;
            };
        }

    }
    
    public void CalculateValues(Vector3 destination)
    {      
        distance = Vector3.Distance(transform.position, destination);
        moveDuration = Mathf.Clamp(distance / 20f, 1.5f * speedRatio, 3.5f *speedRatio);

        angle = Vector3.Angle(transform.forward, destination - transform.position);
        rotDuration = Mathf.Clamp(angle * 0.02f, 0.5f * speedRatio, 1.5f * speedRatio);

    }

    public void SmoothMovement()
    {
        smoothSpeed += 1f;
        smoothSpeed = Mathf.Clamp(smoothSpeed, 0f, maxSpeed);

        if (angle > 90f)
        {
            transform.Translate(smoothSpeed * 0.2f * Time.deltaTime * 1f * -Vector3.forward, Space.Self);
        }
        else
        {
            transform.Translate(smoothSpeed * 0.2f * Time.deltaTime * 1f * Vector3.forward, Space.Self);
        }

        
    }

    public void ResetSmoothValue()
    {
        smoothSpeed = 0f;
    }

}