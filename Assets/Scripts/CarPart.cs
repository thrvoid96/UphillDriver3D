using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarPart : MonoBehaviour, IPooledObject
{
    [SerializeField] private float respawnTime;
    private Transform blockModelTransform;
    private Material lastMat;
    private string lastTag;

    private Vector3 startPos;

    [HideInInspector] public int playerNum;

    public void onObjectSpawn()
    {
        startPos = transform.position;
        
        LevelManager.instance.GiveBrickPrefabFromGameColor(LevelHolder.instance.colorsInThisLevelList[playerNum], gameObject, playerNum);
        
        lastMat = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
        lastTag = gameObject.tag;

        blockModelTransform = transform.GetChild(0).GetChild(0).transform;
    }

    private IEnumerator respawnCube()
    {
        yield return new WaitForSeconds(respawnTime);
        var objToSpawn = ObjectPooler.instance.SpawnFromPool("Part", startPos, Quaternion.identity, playerNum);
        
        objToSpawn.tag = lastTag;
        objToSpawn.transform.GetChild(0).GetChild(0).tag = lastTag;
        objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = lastMat;
        
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