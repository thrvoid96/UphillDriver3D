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
        protected bool isOnRamp, isInMidSection;
        public bool isCollided;

        
        protected float smoothSpeed;

        private int gridIndex;
        private float startSpeed;

        public float speedRatio;
        
        public int getCurrentGrid
        {
            get => gridIndex;
        }

        public float getMaxSpeed
        {
            get => maxSpeed;
            set => maxSpeed = value;
        }


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
            startSpeed = maxSpeed;
            ReCalculateSpeedRatio();
        }

        protected virtual void Update()
        {

        }

        public void ReCalculateSpeedRatio()
        {
            speedRatio = startSpeed / maxSpeed;
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
                EnterMiddleSection();
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
                        
                        carPartCollector.CalculatePartsAfterCollision();

                        if (!isOnRamp)
                        {
                            CollisionCalculate(enemyCar);

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
                        
                        CollisionCalculate(enemyCar);
                        ShakeCalculate();
                    }
                }
            }

        }

        public void GetDownFromRamp()
        {
            var distance = Vector3.Distance(rampStartPos, transform.position);
            var moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);

            canMove = false;

            transform.DOMove(rampStartPos, moveDuration * speedRatio).SetEase(Ease.InOutSine).OnComplete(ExitRamp);
        }

        private void CollisionCalculate(CommonBehaviours enemyCar)
        {
            var direction = transform.position - enemyCar.transform.position;
            collisionFinalPos = transform.position + new Vector3(direction.x * 1.3f * (1f / enemyCar.speedRatio), 0,
                direction.z * 1.3f * (1f / enemyCar.speedRatio));

            transform.DOMove(collisionFinalPos, 0.5f * enemyCar.speedRatio).SetEase(Ease.InOutSine)

                .OnComplete(() => { isCollided = false; })
                .OnKill(() => { isCollided = false; });
        }

        private void ShakeCalculate()
        {
            transform.DOShakeRotation(0.4f, 10f, 2, 10f, true)
                .OnKill(() =>
                {
                    transform.DORotateQuaternion(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), 0f);
                });
        }

        private void SetRampValues(Vector3 closestPoint, Transform colliderTrans)
        {
            var position = colliderTrans.position;
            rampHeight = 2 * (position.y - closestPoint.y);
            rampLength = 2 * (position.z - closestPoint.z);
            rampAngleX = Mathf.Abs(360 - colliderTrans.eulerAngles.x);
        }

        private void EnterMiddleSection()
        {
            if (!canMove) return;
            canMove = false;
            isInMidSection = true;

            transform.DOKill();

            var enteranceAngle = Vector3.Angle(transform.forward, Vector3.forward);
            var duration = Mathf.Clamp(enteranceAngle * 0.02f, 0.1f, 1f);

            var posToGo = transform.position + new Vector3(0, 0, 20f);
            
            transform.DOLookAt(posToGo, duration * speedRatio).OnUpdate(() =>
                {
                    smoothSpeed += 1f;
                    smoothSpeed = Mathf.Clamp(smoothSpeed, 0, maxSpeed);
                    transform.Translate(smoothSpeed * 0.2f * Time.deltaTime * 1f * -Vector3.forward, Space.Self);
                })
                .OnComplete(() =>
                {
                    transform.DOMove(posToGo, 0.75f * speedRatio).SetEase(Ease.InOutSine);

                    smoothSpeed = 0f;
                });
        }

        private void EnterRamp(Collider other)
        {
            transform.DOKill();
            
            transform.DOLookAt(other.transform.position + new Vector3(transform.position.x - other.transform.position.x, 0.573f, 0), 0.75f * speedRatio).SetEase(Ease.InOutSine)
            .OnStart(() =>
            {
                var startDest = new Vector3(transform.position.x, other.transform.position.y - (rampHeight * 0.45f), other.transform.position.z - (rampLength * 0.45f));

                transform.DOMove(startDest, 0.75f * speedRatio).SetEase(Ease.InOutSine);

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(other.transform.rotation, 0.1f).OnComplete(() =>
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

        public void ExitRamp()
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

            transform.DOLookAt(transform.position + transform.right * random, 0.75f * speedRatio).SetEase(Ease.InOutSine)
            .OnStart(() =>
            {
                isInMidSection = true;
                
                canMove = false;

                transform.DOMove(new Vector3(transform.position.x + (-random * 10f), transform.position.y - (rampHeight * 0.05f), transform.position.z - 10f), 0.75f * speedRatio).SetEase(Ease.InOutSine);

            })
            .OnComplete(() =>
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), 0.1f)
                .OnComplete(() =>
                {
                    transform.DOLookAt(transform.position - Vector3.forward, 0.75f * speedRatio).SetEase(Ease.InOutSine)
                .OnStart(() =>
                {
                    transform.DOMove(new Vector3(transform.position.x + (random * 10f), transform.position.y, transform.position.z - 10f), 0.75f * speedRatio).SetEase(Ease.InOutSine)
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

        public void ExitRampComplete()
        {
            var goToPos = new Vector3(transform.position.x, transform.position.y + 2.1f, transform.position.z + 10f);

            transform.DOLookAt(goToPos + new Vector3(0,0,20f), 0.75f * speedRatio).OnStart(() =>
            {
                canMove = false;
                
                isInMidSection = true;

                transform.DOMove(goToPos + new Vector3(0, 0, -3f), 0.5f * speedRatio).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    if(gridIndex < LevelHolder.instance.howManyFloors.Count - 1)
                    {
                        gridIndex++;
                        
                        LevelHolder.instance.SpawnAllPartsForPlayer(playerNum, gridIndex);
                    }

                    transform.DOMove(goToPos + new Vector3(0, 0, 10f), 0.5f * speedRatio).SetEase(Ease.InOutSine).OnComplete(() =>
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