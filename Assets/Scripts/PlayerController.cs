using System.Collections;
using System.Collections.Generic;
using Behaviours;
using DG.Tweening;
using UnityEngine;

public class PlayerController : CommonBehaviours
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
            
        StartCoroutine("Movement");
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    private IEnumerator Movement()
    {
        while (true)
        {
            if (LevelManager.gameState == GameState.Normal && canMove)
            {
                float verticalInput = Input.GetAxis("Vertical");
                float horizontalInput = Input.GetAxis("Horizontal");

                if (!isOnRamp)
                {
                    transform.Translate(currentSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
                    transform.Rotate(0, horizontalInput * verticalInput * 2f, 0, Space.Self);
                    yield return null;
                }
                else
                {
                    var clampValue = Mathf.Clamp(2f + (0.02f * carPartCollector.collectedPartsCount), 0.1f, 2f);
                    currentSpeed = Mathf.Clamp(currentSpeed - (clampValue / rampAngleX * verticalInput * rampClimbSmoothValue), 0, maxSpeed);
                    transform.Translate(currentSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
                    yield return null;
                }

            }
            else
            {
                yield return null;
            }
        }
    }   
}
