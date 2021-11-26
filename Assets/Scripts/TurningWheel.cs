using System.Collections;
using System.Collections.Generic;
using Behaviours;
using DG.Tweening;
using UnityEngine;

public class TurningWheel : MonoBehaviour
{
    // Start is called before the first frame update

    private void TurnWheelLeft()
    {
        transform.DOLocalRotateQuaternion(Quaternion.Euler(0,0,30f),1f);
    }
    
    private void TurnWheelRight()
    {
        transform.DOLocalRotateQuaternion(Quaternion.Euler(0,0,-30f),1f);
    }
    
    private void IdleWheel()
    {
        transform.DOLocalRotateQuaternion(Quaternion.Euler(0,0,0f),1f);
    }
}
