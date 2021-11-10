using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float currentSpeed;

    private bool isOnRamp;   
    private float rampHeight;
    private float rampAngleX;

    private float lastPositionY;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = maxSpeed;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, -transform.up * 3f, Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.gameState == GameState.Normal)
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            transform.Rotate(0, horizontalInput * verticalInput, 0, Space.Self);
                       
            if(isOnRamp)
            {
                Debug.LogError(transform.eulerAngles.y);
                if ((transform.eulerAngles.y >= 0 && transform.eulerAngles.y <= 90 || transform.eulerAngles.y >= 270 && transform.eulerAngles.y <= 360) && verticalInput <= 0)
                {
                    transform.Translate(maxSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
                }
                else if (transform.eulerAngles.y > 90 && transform.eulerAngles.y < 270 && verticalInput > 0)
                {
                    transform.Translate(maxSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
                }
                else
                {
                    transform.Translate(currentSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
                }

                if (lastPositionY < transform.position.y)
                {
                    Debug.LogError("goingUp");
                    currentSpeed = Mathf.Clamp(currentSpeed - (2 / rampHeight * rampAngleX * (transform.position.y - lastPositionY)), 0, maxSpeed);
                   
                }
                else if(lastPositionY > transform.position.y)
                {
                    Debug.LogError("goingDown");
                    currentSpeed = Mathf.Clamp(currentSpeed + (2 / rampHeight * rampAngleX * (lastPositionY - transform.position.y)), 0, maxSpeed);
                }
               
            }
            else
            {
                transform.Translate(currentSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);
            }

            lastPositionY = transform.position.y;
        }            

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Ramp"))
        {
            transform.rotation = Quaternion.LookRotation(other.transform.forward);

            rampHeight = 2 * (other.transform.position.y - other.ClosestPoint(transform.position).y);
            rampAngleX = Mathf.Abs(360 - other.transform.eulerAngles.x);

            isOnRamp = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ramp"))
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
            currentSpeed = maxSpeed;
            isOnRamp = false;
        }
    }
}
