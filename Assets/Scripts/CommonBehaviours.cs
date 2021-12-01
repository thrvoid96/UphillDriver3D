using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using Random = UnityEngine.Random;

//Combine same behaviours between the Player and AI.

namespace Behaviours
{
    public abstract class CommonBehaviours : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] protected int playerNum;       
        [SerializeField] protected float maxSpeed;
        [SerializeField] protected CarPartCollector carPartCollector;
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
        [System.NonSerialized] public List<TrailRenderer> currentTrails = new List<TrailRenderer>();
        [System.NonSerialized] public List<GameObject> currentWheels = new List<GameObject>();
        
        private List<ParticleSystem> smokeEffects = new List<ParticleSystem>();
        
        protected Vector3 collisionFinalPos;

        protected bool canMove = true;
        protected bool isOnRamp; 
        protected bool isCollided;


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
            
            GetSmokeEffects();
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
                
                    if (enemyCar.carPartCollector.collectedPartsCount > carPartCollector.collectedPartsCount)
                    { 
                        transform.DOKill();
                        
                        isCollided = true;
                        canMove = false;
                        
                        carPartCollector.CalculatePartsAfterCollision();

                        if (!isOnRamp)
                        {
                            CollisionWithCar(enemyCar);

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
                        canMove = false;
                        
                        CollisionWithCar(enemyCar);
                        ShakeCalculate();
                    } 
            }
            
            else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                if (other.CompareTag("BackWall"))
                {
                    if (transform.position.z > other.transform.position.z)
                    {
                        transform.DOKill();
                        isCollided = true;
                        canMove = false;

                        CollisionWithWall(other);
                        ShakeCalculate();
                    }
                }
                else
                {
                    transform.DOKill();
                    isCollided = true;
                    canMove = false;

                    CollisionWithWall(other);
                    ShakeCalculate();
                }
                
            }

        }

        private void GetSmokeEffects()
        {
            for (int i = 0; i < transform.GetChild(transform.childCount-1).childCount; i++)
            {
                smokeEffects.Add(transform.GetChild(transform.childCount-1).GetChild(i).GetComponent<ParticleSystem>());
                smokeEffects[i].Stop();
            }
        }
        
        public void GetDownFromRamp()
        {
            var distance = Vector3.Distance(rampStartPos, transform.position);
            var moveDuration = Mathf.Clamp(distance / 20f, 1.5f, 3.5f);

            canMove = false;
            
            transform.DOMove(rampStartPos, moveDuration * speedRatio).SetEase(Ease.InOutSine).OnComplete(ExitRamp);
        }

        private void CollisionWithCar(CommonBehaviours enemyCar)
        {
            var direction = transform.position - enemyCar.transform.position;
            collisionFinalPos = transform.position + new Vector3(direction.x * 1.3f * (1f / enemyCar.speedRatio), 0,
                direction.z * 1.3f * (1f / enemyCar.speedRatio));

            transform.DOMove(collisionFinalPos, 0.5f * enemyCar.speedRatio).SetEase(Ease.InOutSine)

                .OnComplete(() => { isCollided = false; canMove = true; })
                .OnKill(() => { isCollided = false;});
        }
        
        private void CollisionWithWall(Collider other)
        {
            var direction = other.transform.forward;

            collisionFinalPos = transform.position + new Vector3(direction.x * 1.3f * (1f / speedRatio), 0,
                direction.z * 1.3f * (1f / speedRatio));

            transform.DOMove(collisionFinalPos, 0.5f * speedRatio).SetEase(Ease.InOutSine)

                .OnComplete(() => { isCollided = false; canMove = true; })
                .OnKill(() => { isCollided = false;});
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
            
            transform.DOKill();
            
            for (int i = 0; i < currentWheels.Count; i++)
            {
                currentWheels[i].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 1f);
            }

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
                
                isOnRamp = true;

                transform.DOMove(startDest, 0.75f * speedRatio).SetEase(Ease.InOutSine);

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(other.transform.rotation, 0.1f).OnComplete(() =>
                {
                    rampStartPos = transform.position;

                    coefficient = Mathf.Clamp(((float) carPartCollector.collectedPartsCount / LevelHolder.instance.howManyFloors[gridIndex].blocksToPassRamp)* 0.9f, 0.1f, 0.9f);

                    finalPos = rampStartPos + new Vector3(0, (rampHeight * coefficient) + 0.573f, rampLength * coefficient);
                    
                    canMove = true;

                }); ;
            });
        }

        public void ExitRamp()
        {
            var random = 0;
            if (Random.Range(1, 3) > 1)
            {
                random = -1;
            }
            else
            {
                random = 1;
            }

            if (Physics.Raycast(transform.position,transform.right,15f,LayerMask.GetMask("Wall"),QueryTriggerInteraction.Collide))
            {
                random = 1;
            } 
            else if (Physics.Raycast(transform.position, -transform.right, 15f, LayerMask.GetMask("Wall"), QueryTriggerInteraction.Collide))
            {
                random = -1;
            }

            transform.DOLookAt(transform.position + transform.right * random, 0.75f * speedRatio).SetEase(Ease.InOutSine)
            .OnStart(() =>
            {
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
                    transform.DOMove(new Vector3(transform.position.x + (random * 10f), transform.position.y, transform.position.z - 15f), 0.75f * speedRatio).SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        canMove = true;

                        isOnRamp = false;
                        
                        isCollided = false;
                    }).OnKill(() =>
                    {
                        canMove = true;

                        isOnRamp = false;
                        
                        isCollided = false;
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

                transform.DOMove(goToPos + new Vector3(0, 0, -3f), 0.5f * speedRatio).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    if(gridIndex < LevelHolder.instance.howManyFloors.Count - 1)
                    {

                        gridIndex++;
                        
                        carPartCollector.CalculatePartsAfterRamp(LevelHolder.instance.howManyFloors[getCurrentGrid].blocksToPassRamp);
                        
                        LevelHolder.instance.SpawnAllPartsForPlayer(playerNum, gridIndex);
                    }

                    transform.DOMove(goToPos + new Vector3(0, 0, 10f), 0.5f * speedRatio).SetEase(Ease.InOutSine).OnComplete(() =>
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

        public void StartTrails()
        {
            foreach(TrailRenderer trail in currentTrails)
            {
                trail.emitting = true;
            }
        }

        public void StopTrails()
        {
            foreach (TrailRenderer trail in currentTrails)
            {
                trail.emitting = false;
            }
        }

        public void StartSmokes()
        {
            foreach (ParticleSystem smoke in smokeEffects)
            {
                smoke.Play();
            }
        }

        public void StopSmokes()
        {
            foreach (ParticleSystem smoke in smokeEffects)
            {
                smoke.Stop();
            }
        }
        
        public void ReCalculateSpeedRatio()
        {
            speedRatio = startSpeed / maxSpeed;
        }

        public void RotateWheelsAfterVehicleChange()
        {
            var horizontalInput = Input.GetAxis("Horizontal");

            if (horizontalInput > 0)
            {
                for (int i = 0; i < currentWheels.Count; i++)
                {
                    currentWheels[i].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 25), 0f).SetEase(Ease.Flash);
                }
                
                StartTrails();
            }
            else if (horizontalInput == 0)
            {
                for (int i = 0; i < currentWheels.Count; i++)
                {
                    currentWheels[i].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 0f).SetEase(Ease.Flash);
                }
            }
            else
            {
                for (int i = 0; i < currentWheels.Count; i++)
                {
                    currentWheels[i].transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -25), 0f).SetEase(Ease.Flash);
                }
                
                StartTrails();
            }
        }
        
    }

}