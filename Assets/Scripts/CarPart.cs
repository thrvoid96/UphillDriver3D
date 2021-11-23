﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarPart : MonoBehaviour, IPooledObject
{
    [SerializeField] private float respawnTime;
    private Transform blockModelTransform;

    private Vector3 startPos;

    public void onObjectSpawn()
    {
        startPos = transform.position;
        blockModelTransform = transform.GetChild(0).GetChild(0).transform;
    }

    private IEnumerator respawnCube()
    {
        yield return new WaitForSeconds(respawnTime);
        ObjectPooler.instance.SpawnFromPool(gameObject.tag, startPos, Quaternion.identity);
        gameObject.SetActive(false);
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(gameObject.tag))
        {
            StartCoroutine(respawnCube());
            blockModelTransform.gameObject.SetActive(false);
        }
        
    }

}