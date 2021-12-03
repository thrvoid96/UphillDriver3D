using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour, IPooledObject
{
    public int currentGrid;
    private BoxCollider boxCollider;
    private int count1,count2;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void onObjectSpawn()
    {

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Part"))
        {
            var part = other.GetComponent<CarPart>();

            LevelHolder.instance.howManyFloors[currentGrid].blockPositions[part.playerNum].RemoveAt(part.blockIndexInList);

            part.DisableSpawn();

            count1++;
            
            Debug.LogError(count1);
            Debug.LogError(count2);
            
            if (count1 == count2)
            {
                boxCollider.size = new Vector3(1,0,1);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.LogError(other.tag);
        if (other.gameObject.layer == LayerMask.NameToLayer("Part"))
        {
            count2++;
        }
    }
}
