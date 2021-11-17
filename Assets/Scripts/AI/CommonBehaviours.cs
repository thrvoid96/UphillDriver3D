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
        [SerializeField] private int playerNum;      
        [SerializeField] protected float maxSpeed;
        [SerializeField] protected float currentSpeed;

        public List<Floors> floorsOnScene = new List<Floors>();
        

        [System.Serializable]
        public class Floors
        {
            public List<int> gridIndexes = new List<int>();
        }
        #endregion


        #region Components       

        protected Animator animator;
        protected CarPartCollector carPartCollector;

        #endregion


        #region Variables

        protected bool canMove = true;
        protected bool isOnRamp;
        protected bool enteringRamp;


        protected float rampHeight, rampLength, rampAngleX;
        protected float specialSpeed;


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

                SetRampValues(closestPoint, other.transform);
                EnterRamp(other.transform);

            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("MiddleSect"))
            {
                if (currentSpeed == maxSpeed)
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

        }

        private void SetRampValues(Vector3 closestPoint, Transform colliderTrans)
        {
            rampHeight = 2 * (colliderTrans.position.y - closestPoint.y);
            rampLength = 2 * (colliderTrans.position.z - closestPoint.z);
            rampAngleX = Mathf.Abs(360 - colliderTrans.eulerAngles.x);
        }

        private void EnterMiddleSection(Transform colliderTrans)
        {
            if (canMove)
            {
                canMove = false;

                float enteranceAngle = Vector3.Angle(transform.forward, colliderTrans.transform.position - transform.position);
                float duration = Mathf.Clamp(enteranceAngle * 0.02f, 0.1f, 1f);

                Vector3 finalPos = new Vector3(colliderTrans.transform.position.x, transform.position.y, colliderTrans.transform.position.z);

                transform.DOLookAt(finalPos, duration).OnUpdate(() =>
                {
                    specialSpeed += 1f;
                    float finalSpeed = Mathf.Clamp(specialSpeed, 0, maxSpeed);
                    transform.Translate(finalSpeed * 0.2f * Time.deltaTime * 1f * -Vector3.forward, Space.Self);
                })
                .OnComplete(() =>
                {
                    transform.DOMove(finalPos, 2f).SetEase(Ease.InOutSine);
                    specialSpeed = 0f;
                });
            }
        }

        private void EnterRamp(Transform colliderTrans)
        {
            transform.DOLookAt(colliderTrans.position, 1f, AxisConstraint.None).OnStart(() =>
            {
                Vector3 finalPos = new Vector3(colliderTrans.position.x, colliderTrans.position.y - (rampHeight * 0.45f), colliderTrans.position.z - (rampLength * 0.45f));

                transform.DOMove(finalPos, 2f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    isOnRamp = true;

                    canMove = true;

                });

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(colliderTrans.rotation, 0.5f);
            });
        }

        private void ExitRamp(Transform colliderTrans)
        {
            transform.DOLookAt(colliderTrans.position + new Vector3(0, 0.573f, 20f), 1f, AxisConstraint.None, transform.up).OnStart(() =>
            {
                canMove = false;

                transform.DOMove(colliderTrans.position + new Vector3(0, 0.573f, -15f), 2f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    isOnRamp = false;

                    canMove = true;

                    currentSpeed = maxSpeed;
                });

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.5f);
            });
        }

        private void ExitRampComplete(Transform colliderTrans)
        {
            transform.DOLookAt(colliderTrans.position + new Vector3(0, 0.573f, 20f), 1.5f).OnStart(() =>
            {
                canMove = false;

                transform.DOMove(colliderTrans.position + new Vector3(0, 0.7f, -3f), 1f).SetEase(Ease.InOutSine).OnComplete(() =>
                {

                    transform.DOMove(colliderTrans.position + new Vector3(0, 0.573f, 10f), 1f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {

                        isOnRamp = false;

                        canMove = true;

                        currentSpeed = maxSpeed;
                    });

                });

            }).OnComplete(() =>
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.5f);
            });
        }

    }           
    
}