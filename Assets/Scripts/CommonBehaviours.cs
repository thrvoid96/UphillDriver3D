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
        public GameColor color;

        #endregion


        #region Components       

        protected Animator animator;
        
        #endregion


        #region Variables

        [System.NonSerialized] public float rampHeight, rampLength, rampAngleX, coefficient;
        [System.NonSerialized] public Vector3 finalPos;
        [System.NonSerialized] public Vector3 rampStartPos;
        public Vector3 collisionFinalPos;

        protected bool canMove = true;
        protected bool isOnRamp, isInMidSection, enteringRamp;
        public bool isCollided;

        
        protected float smoothSpeed;

        private int gridIndex;
        
        public int getCurrentGrid
        {
            get => gridIndex;
            set => gridIndex = value;
        }

        public float getMaxSpeed => maxSpeed;

        public int getPlayerNum  
        {
            get => playerNum;
            set => playerNum = value;
        }

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

                SetRampValues(closestPoint, other.transform);
                
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
                
                if (playerNum != 0)
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
                
                if(!isInMidSection){
                    
                    if (enemyCar.carPartCollector.collectedPartsCount > carPartCollector.collectedPartsCount)
                    { 
                        transform.DOKill();
                        isCollided = true;
                        carPartCollector.DowngradeCar();

                        if (!isOnRamp)
                        {
                            CollisionCalculate(other);

                            ShakeCalculate();

                        }
                        else
                        {
                            GetDownFromRamp();
                        }
                    }
                    else if(enemyCar.carPartCollector.collectedPartsCount == carPartCollector.collectedPartsCount)
                    {
                        transform.DOKill();
                        isCollided = true;
                        
                        CollisionCalculate(other);
                        ShakeCalculate();
                    }
                }
            }

        }

        private void GetDownFromRamp()
        {
            var distance =
                Vector3.Distance(
                    rampStartPos + new Vector3(0, -rampHeight * 0.05f, -rampLength * 0.05f),
                    transform.position);
            var moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);

            transform.DOMove(rampStartPos + new Vector3(0, -rampHeight * 0.05f, -rampLength * 0.05f),
                moveDuration).SetEase(Ease.InOutSine).OnComplete(
                () => { isCollided = false; });
        }

        private void CollisionCalculate(Collider other)
        {
            var direction = transform.position - other.transform.position;
            collisionFinalPos = transform.position +
                                new Vector3(direction.x * 1.2f, 0, direction.z * 1.2f);

            transform.DOMove(collisionFinalPos, 0.5f).SetEase(Ease.InOutSine)

                .OnComplete(() => { isCollided = false; });
        }

        private void ShakeCalculate()
        {
            var rotateValue = Quaternion.Euler(10, transform.eulerAngles.y, 10);

            if (transform.position.x >= collisionFinalPos.x)
            {
                if (transform.position.z >= collisionFinalPos.z)
                {
                    rotateValue.x = rotateValue.x;
                    rotateValue.z = -rotateValue.z;
                }
                else
                {
                    rotateValue.x = -rotateValue.x;
                    rotateValue.z = -rotateValue.z;
                }
            }
            else
            {
                if (transform.position.z >= collisionFinalPos.z)
                {
                    rotateValue.x = rotateValue.x;
                    rotateValue.z = rotateValue.z;
                }
                else
                {
                    rotateValue.x = -rotateValue.x;
                    rotateValue.z = rotateValue.z;
                }
            }

            transform.DORotateQuaternion(rotateValue, 0.25f).SetEase(Ease.InOutSine).OnComplete(
                () =>
                {
                    transform.DORotateQuaternion(Quaternion.Euler(0, transform.eulerAngles.y, 0), 0.25f)
                        .SetEase(Ease.InOutSine);
                });
        }

        private void SetRampValues(Vector3 closestPoint, Transform colliderTrans)
        {
            var position = colliderTrans.position;
            rampHeight = 2 * (position.y - closestPoint.y);
            rampLength = 2 * (position.z - closestPoint.z);
            rampAngleX = Mathf.Abs(360 - colliderTrans.eulerAngles.x);
        }

        private void EnterMiddleSection(Transform colliderTrans)
        {
            if (!canMove) return;
            canMove = false;

            isInMidSection = true;

            var enteranceAngle = Vector3.Angle(transform.forward, Vector3.forward);
            var duration = Mathf.Clamp(enteranceAngle * 0.02f, 0.1f, 1f);

            var posToGo = transform.position + new Vector3(0, 0, 20f);
            
            transform.DOLookAt(posToGo, duration).OnUpdate(() =>
                {
                    smoothSpeed += 1f;
                    smoothSpeed = Mathf.Clamp(smoothSpeed, 0, maxSpeed);
                    transform.Translate(smoothSpeed * 0.2f * Time.deltaTime * 1f * -Vector3.forward, Space.Self);
                })
                .OnComplete(() =>
                {
                    transform.DOMove(posToGo, 0.75f).SetEase(Ease.InOutSine);

                    smoothSpeed = 0f;
                });
        }

        private void EnterRamp(Collider collider)
        {
            transform.DOLookAt(collider.transform.position + new Vector3(transform.position.x - collider.transform.position.x, 0.573f, 0), 0.75f).SetEase(Ease.InOutSine)
            .OnStart(() =>
            {
                var startDest = new Vector3(transform.position.x, collider.transform.position.y - (rampHeight * 0.45f), collider.transform.position.z - (rampLength * 0.45f));

                transform.DOMove(startDest, 0.75f).SetEase(Ease.InOutSine);

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(collider.transform.rotation, 0.1f).OnComplete(() =>
                {
                    rampStartPos = transform.position;

                    coefficient = Mathf.Clamp(((float) carPartCollector.collectedPartsCount / LevelHolder.instance.howManyFloors[gridIndex].blocksToPassRamp)* 0.9f, 0.1f, 0.9f);

                    finalPos = rampStartPos + new Vector3(0, (rampHeight * coefficient) + 0.573f, rampLength * coefficient);
                    
                    isInMidSection = false;
                    
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
                isInMidSection = true;
                
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
                    transform.DOMove(new Vector3(transform.position.x + (random * 10f), colliderTrans.position.y + 0.573f, colliderTrans.position.z - 10f), 0.75f).SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        canMove = true;

                        isOnRamp = false;
                        
                        isCollided = false;
                        
                        isInMidSection = false;
                    });

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
                
                isInMidSection = true;

                transform.DOMove(goToPos + new Vector3(0, 0, -3f), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    if(gridIndex < LevelHolder.instance.howManyFloors.Count - 1)
                    {
                        gridIndex++;
                        
                        LevelHolder.instance.SpawnAllPartsForPlayer(playerNum, gridIndex);
                    }

                    transform.DOMove(goToPos + new Vector3(0, 0, 10f), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {                     
                        isInMidSection = false;
                        
                        isOnRamp = false;

                        canMove = true;
                    });

                });

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.1f);
            });
            
        }

        public void StartTrails()
        {
            foreach(TrailRenderer trail in allTrails)
            {
                trail.emitting = true;
            }

            StartCoroutine(nameof(CloseDelay));
        }

        public void StopTrails()
        {
            foreach (TrailRenderer trail in allTrails)
            {
                trail.emitting = false;
            }
        }

        private IEnumerator CloseDelay()
        {
            yield return new WaitForSeconds(trailStayTime);
            StopTrails();
            yield break;
        }

        

    }

}