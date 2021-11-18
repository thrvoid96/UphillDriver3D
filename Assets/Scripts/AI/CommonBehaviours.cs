using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

//Combine same behaviours between the Player and AI.

namespace Behaviours
{
    public abstract class CommonBehaviours : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] protected int playerNum;       
        [SerializeField] protected float maxSpeed;
        [SerializeField] protected float currentSpeed;
        public float rampClimbSmoothValue;

        

        #endregion


        #region Components       

        protected Animator animator;
        protected CarPartCollector carPartCollector;
        protected Ramp currentRamp;

        #endregion


        #region Variables

        [System.NonSerialized] public float rampHeight, rampLength, rampAngleX, coefficient;
        [System.NonSerialized] public Vector3 finalPos;
        [System.NonSerialized] public Vector3 rampStartPos;

        protected bool canMove = true;
        protected bool isOnRamp;
        protected bool enteringRamp;
        
        protected float smoothSpeed;

        private int gridIndex;
       
        public int getCollectedAmount
        {
            get { return carPartCollector.collectedPartsCount; }
        }

        public int getCurrentGrid
        {
            get { return gridIndex; }
            set { gridIndex = value; }
        }

        public float getCurrentSpeed
        {
            get { return currentSpeed; }
            set { currentSpeed = value; }
        }

        public float getMaxSpeed
        {
            get { return maxSpeed; }
        }

        public int getPlayerNum
        {
            get { return playerNum; }
        }


        #endregion


        protected virtual void Awake()
        {
            animator = transform.parent.GetComponent<Animator>();
            carPartCollector = transform.GetChild(0).GetComponent<CarPartCollector>();
        }

        protected virtual void Start()
        {
            currentSpeed = maxSpeed;
        }

        protected virtual void Update()
        {

        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ramp"))
            {
                var closestPoint = other.ClosestPoint(transform.position);
                var ramp = other.GetComponent<Ramp>();

                SetRampValues(closestPoint, other.transform, ramp);
                
                EnterRamp(other);

            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("MiddleSect"))
            {
                if (transform.position.y > other.transform.position.y)
                {
                    enteringRamp = true;
                }
                else
                {
                    enteringRamp = false;
                }

                if (transform.position.z < other.transform.position.z)
                {
                    if (enteringRamp)
                    {
                        EnterMiddleSection(other.transform);
                    }
                    else
                    {
                        ExitRampComplete(other.transform);
                    }

                }

                else
                {
                    ExitRamp(other.transform);
                }


            }

            else if(other.gameObject.layer == LayerMask.NameToLayer("Finish"))
            {
                transform.DOKill();

                LevelManager.gameState = GameState.Finish;

                if (gameObject.transform.GetChild(0).gameObject.layer != LayerMask.NameToLayer("Part0Collector"))
                {

                    LevelManager.instance.Fail();
                }
                else
                {                   

                    LevelManager.instance.Victory();
                }
                
            }

        }

        private void SetRampValues(Vector3 closestPoint, Transform colliderTrans,Ramp ramp)
        {
            rampHeight = 2 * (colliderTrans.position.y - closestPoint.y);
            rampLength = 2 * (colliderTrans.position.z - closestPoint.z);
            rampAngleX = Mathf.Abs(360 - colliderTrans.eulerAngles.x);

            currentRamp = ramp;
        }

        private void EnterMiddleSection(Transform colliderTrans)
        {
            if (canMove)
            {
                canMove = false;

                float enteranceAngle = Vector3.Angle(transform.forward, Vector3.forward);
                float duration = Mathf.Clamp(enteranceAngle * 0.02f, 0.1f, 1f);

                Vector3 finalPos = transform.position + new Vector3(0, 0, 20f);

                transform.DOLookAt(finalPos, duration).OnUpdate(() =>
                {
                    smoothSpeed += 1f;
                    smoothSpeed = Mathf.Clamp(smoothSpeed, 0, maxSpeed);
                    transform.Translate(smoothSpeed * 0.2f * Time.deltaTime * 1f * -Vector3.forward, Space.Self);
                })
                .OnComplete(() =>
                {
                    transform.DOMove(finalPos, 0.75f).SetEase(Ease.InOutSine);
                    smoothSpeed = 0f;
                });
            }
        }

        private void EnterRamp(Collider collider)
        {
            transform.DOLookAt(collider.transform.position + new Vector3(transform.position.x - collider.transform.position.x, 0.573f, 0), 0.75f, AxisConstraint.None).SetEase(Ease.InOutSine)
            
                .OnStart(() =>
            {
                Vector3 finalPos = new Vector3(transform.position.x, collider.transform.position.y - (rampHeight * 0.45f), collider.transform.position.z - (rampLength * 0.45f));

                transform.DOMove(finalPos, 0.75f).SetEase(Ease.InOutSine);

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(collider.transform.rotation, 0.1f).OnComplete(() =>
                {
                    rampStartPos = transform.position;

                    coefficient = Mathf.Clamp((float) carPartCollector.collectedPartsCount / currentRamp.getBlocksNeededToClimb, 0.1f, 0.9f);

                    finalPos = rampStartPos + new Vector3(0, rampHeight * coefficient, rampLength * coefficient);

                    isOnRamp = true;

                    canMove = true;

                }); ;
            });
        }

        private void ExitRamp(Transform colliderTrans)
        {
            var random = 0;
            if (Random.Range(0, 2) > 0)
            {
                random = -1;
            }
            else
            {
                random = 1;
            }

            transform.DOLookAt(transform.position + transform.right * random, 0.75f).SetEase(Ease.InOutSine)
            .OnStart(() =>
            {
                canMove = false;

                transform.DOMove(new Vector3(transform.position.x + (-random * 10f), colliderTrans.position.y + 0.573f, colliderTrans.position.z), 0.75f).SetEase(Ease.InOutSine);


            })
            .OnComplete(() =>
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), 0.1f)
                .OnComplete(() =>
                {
                    transform.DOLookAt(transform.position - Vector3.forward, 0.75f).SetEase(Ease.InOutSine)
                .OnStart(() =>
                {
                    canMove = false;

                    transform.DOMove(new Vector3(transform.position.x + (random * 10f), colliderTrans.position.y + 0.573f, colliderTrans.position.z - 10f), 0.75f).SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        isOnRamp = false;

                        canMove = true;

                        currentSpeed = maxSpeed;
                    }); ;


                });


                });
            });
        }

        private void ExitRampComplete(Transform colliderTrans)
        {
            Vector3 finalPos = new Vector3(transform.position.x, colliderTrans.position.y + 0.573f, colliderTrans.position.z);

            transform.DOLookAt(finalPos+ new Vector3(0,0,20f), 0.75f).OnStart(() =>
            {
                canMove = false;

                transform.DOMove(finalPos + new Vector3(0, 0, -3f), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                {

                    transform.DOMove(finalPos + new Vector3(0, 0, 10f), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        gridIndex++;

                        CollectablePartSpawner.instance.SpawnAllPartsForPlayer(playerNum, gridIndex);

                        isOnRamp = false;

                        canMove = true;

                        currentSpeed = maxSpeed;
                    });

                });

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.1f);
            });
        }

    }



}