using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float currentSpeed;

    private CarPartCollector carPartCollector;

    private bool isOnRamp;
    private float rampHeight, rampLength;
    private float rampAngleX;

    private float lastPositionY;

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = maxSpeed;
        carPartCollector = transform.GetChild(0).GetComponent<CarPartCollector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.gameState == GameState.Normal)
        {
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            transform.Translate(currentSpeed * Time.deltaTime * verticalInput * Vector3.forward, Space.Self);

            if (isOnRamp)
            {
                if (lastPositionY < transform.position.y)
                {
                    Debug.LogWarning("goingDown");
                    var clampValue = Mathf.Clamp(2f - (0.02f * carPartCollector.collectedPartsCount), 0.1f, 2f);
                    currentSpeed = Mathf.Clamp(currentSpeed - (clampValue / 30f * rampAngleX * (transform.position.y - lastPositionY)), 0, maxSpeed);

                }
                else if (lastPositionY > transform.position.y)
                {
                    Debug.LogWarning("goingDown");
                    currentSpeed = Mathf.Clamp(currentSpeed + (4 / 30f * rampAngleX * (lastPositionY - transform.position.y)), 0, maxSpeed);
                }

            }
            else
            {
                transform.Rotate(0, horizontalInput * verticalInput, 0, Space.Self);
            }

            lastPositionY = transform.position.y;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ramp"))
        {
            var closestPoint = other.ClosestPoint(transform.position);
            rampHeight = 2 * (other.transform.position.y - closestPoint.y);
            rampLength = 2 * (other.transform.position.z - closestPoint.z);
            rampAngleX = Mathf.Abs(360 - other.transform.eulerAngles.x);

            transform.position = new Vector3(other.transform.position.x, other.transform.position.y - rampHeight / 2, other.transform.position.z - rampLength / 2);
            transform.rotation = Quaternion.LookRotation(other.transform.forward);

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
