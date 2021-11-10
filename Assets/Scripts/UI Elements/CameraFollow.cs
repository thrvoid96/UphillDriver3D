using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Vector3 firstOffset;
    public bool bigVictory;

    private void Start()
    {
        firstOffset = offset;    
    }
    private void LateUpdate()
    {
        if (LevelManager.gameState != GameState.Failed)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        offset = transform.position - target.transform.position;
    //        Debug.Log("offset " + offset);
    //    }
    //}
}
