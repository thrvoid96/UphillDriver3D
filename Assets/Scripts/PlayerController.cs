using System.Collections;
using System.Collections.Generic;
using Behaviours;
using DG.Tweening;
using UnityEngine;

public class PlayerController : CommonBehaviours
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float currentSpeed;

    private CarPartCollector carPartCollector;

    private bool enteringRamp;
    private bool canMove = true;

    private float rampHeight, rampLength, rampAngleX;
    private float specialSpeed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentSpeed = maxSpeed;
        carPartCollector = transform.GetChild(0).GetComponent<CarPartCollector>();

        StartCoroutine("Movement");
    }

    private void OnTriggerEnter(Collider other)
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

    private IEnumerator Movement()
    {
        while (true)
        {
            if (LevelManager.gameState == GameState.Normal && canMove)
            {
                float verticalInput = Input.GetAxis("Vertical");
                float horizontalInput = Input.GetAxis("Horizontal");

                transform.Translate(currentSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
                transform.Rotate(0, horizontalInput * verticalInput * 2f, 0, Space.Self);

                yield return null;
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator RampMovement()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            float verticalInput = Input.GetAxis("Vertical");

            if (LevelManager.gameState == GameState.Normal && canMove)
            {
                var clampValue = Mathf.Clamp(2f + (0.02f * carPartCollector.collectedPartsCount), 0.1f, 2f);
                currentSpeed = Mathf.Clamp(currentSpeed - (clampValue / 30f * rampAngleX * verticalInput * 0.1f), 0, maxSpeed);
                transform.Translate(currentSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
                yield return null;

            }
            else
            {
                yield return null;
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

            StopCoroutine("Movement");

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
                StartCoroutine("RampMovement");

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

            StopCoroutine("RampMovement");

            transform.DOMove(colliderTrans.position + new Vector3(0, 0.573f, -15f), 2f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                StartCoroutine("Movement");

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
        transform.DOLookAt(colliderTrans.position + new Vector3(0, 0.573f, 10f), 1.5f).OnStart(() =>
        {
            canMove = false;

            StopCoroutine("RampMovement");

            transform.DOMove(colliderTrans.position + new Vector3(0, 0.573f, -3f), 1f).SetEase(Ease.InOutSine).OnComplete(() =>
              {

                  transform.DOMove(colliderTrans.position + new Vector3(0, 0.573f, 10f), 1f).SetEase(Ease.InOutSine).OnComplete(() =>
                  {

                      StartCoroutine("Movement");

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
