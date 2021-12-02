using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    
    private void LateUpdate()
    {
        if (LevelManager.gameState != GameState.Failed)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    public void ChangeCameraZoom(bool zoomIn)
    {
        if (zoomIn)
        {
            offset += new Vector3(0, 10, -10);
        }
        else
        {
            offset -= new Vector3(0, 10, -10);
        }
    }


}
