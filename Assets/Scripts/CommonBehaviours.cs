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
        [SerializeField] protected CarPartCollector carPartCollector;
        [SerializeField] private List<TrailRenderer> allTrails = new List<TrailRenderer>();
        [SerializeField] private float trailStayTime;
        public float rampClimbSmoothValue;

        #endregion


        #region Components       

        protected Animator animator;

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
        
        public int getCurrentGrid
        {
            get => gridIndex;
            set => gridIndex = value;
        }

        public float getMaxSpeed => maxSpeed;

        public int getPlayerNum => playerNum;

        #endregion


        protected virtual void Awake()
        {
            animator = transform.parent.GetComponent<Animator>();
        }

        protected virtual void Start()
        {

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
                        transform.DOKill();

                        ExitRampComplete(other.transform);
                    }

                }

                else
                {
                    transform.DOKill();

                    ExitRamp(other.transform);
                }


            }

            else if(other.gameObject.layer == LayerMask.NameToLayer("Finish"))
            {
                transform.DOKill();

                LevelManager.gameState = GameState.Finish;

                Debug.LogError("here");

                if (gameObject.transform.GetChild(0).gameObject.layer != LayerMask.NameToLayer("Part0Collector"))
                {

                    LevelManager.instance.Fail();
                }
                else
                {                   

                    LevelManager.instance.Victory();
                }
                
            }

            else if (other.gameObject.layer == LayerMask.NameToLayer("Car"))
            {
                var enemyCar = other.GetComponent<CommonBehaviours>();

                if(enemyCar.carPartCollector.collectedPartsCount > carPartCollector.collectedPartsCount)
                {
                    carPartCollector.DowngradeCar();
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
            if (!canMove) return;
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

        private void EnterRamp(Collider collider)
        {
            transform.DOLookAt(collider.transform.position + new Vector3(transform.position.x - collider.transform.position.x, 0.573f, 0), 0.75f).SetEase(Ease.InOutSine)
            .OnStart(() =>
            {
                Vector3 startDest = new Vector3(transform.position.x, collider.transform.position.y - (rampHeight * 0.45f), collider.transform.position.z - (rampLength * 0.45f));

                transform.DOMove(startDest, 0.75f).SetEase(Ease.InOutSine);

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
                    }); ;


                });


                });
            });
        }

        private void ExitRampComplete(Transform colliderTrans)
        {
            var goToPos = new Vector3(transform.position.x, colliderTrans.position.y + 0.573f, colliderTrans.position.z);

            transform.DOLookAt(goToPos + new Vector3(0,0,20f), 0.75f).OnStart(() =>
            {
                canMove = false;

                transform.DOMove(goToPos + new Vector3(0, 0, -3f), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    gridIndex++;

                    if(gridIndex <= CollectablePartSpawner.instance.grids.Count)
                    {
                        CollectablePartSpawner.instance.SpawnAllPartsForPlayer(playerNum, gridIndex);
                    }

                    transform.DOMove(goToPos + new Vector3(0, 0, 10f), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {                      
                        isOnRamp = false;

                        canMove = true;
                    });

                });

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.1f);
            });
        }

        public void startTrails()
        {
            foreach(TrailRenderer trail in allTrails)
            {
                trail.emitting = true;
            }

            StartCoroutine("closeDelay");
        }

        public void stopTrails()
        {
            foreach (TrailRenderer trail in allTrails)
            {
                trail.emitting = false;
            }
        }

        private IEnumerator closeDelay()
        {
            yield return new WaitForSeconds(trailStayTime);
            stopTrails();
            yield break;
        }

        

    }

}