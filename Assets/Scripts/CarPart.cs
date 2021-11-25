using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarPart : MonoBehaviour, IPooledObject
{
    [SerializeField] private float respawnTime;
    private Transform blockModelTransform;
    private BoxCollider boxCol;


    private Vector3 startPos;

    [HideInInspector] public int playerNum;

    private void Start()
    {
        boxCol = GetComponent<BoxCollider>();
    }

    public void onObjectSpawn()
    {
        startPos = transform.position;
        
        LevelManager.instance.GiveBrickPrefabFromGameColor(LevelHolder.instance.colorsInThisLevelList[playerNum], gameObject, playerNum);
        
        blockModelTransform = transform.GetChild(0).GetChild(0).transform;
    }

    private IEnumerator respawnCube()
    {
        yield return new WaitForSeconds(respawnTime);
        boxCol.enabled = true;
        blockModelTransform.gameObject.SetActive(true);
        
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(gameObject.tag))
        {
            StartCoroutine(respawnCube());
            boxCol.enabled = false;
            blockModelTransform.gameObject.SetActive(false);
        }
        
    }

}