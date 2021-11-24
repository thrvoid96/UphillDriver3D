using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPartCollector : MonoBehaviour
{
    public int partCount;
    [SerializeField] private List<SkinnedMeshRenderer> carMeshes = new List<SkinnedMeshRenderer>();
    [SerializeField] private int amountForUpgrade;
    [SerializeField] private int upgradeAmountIncrement;
    
    private Stack<GameObject> collectedParts = new Stack<GameObject>();
    
    [Header("0 for instant, 1 for slowest")]
    [SerializeField] private float blendSmoothSpeed;

    private int inBetween;
    private int currentBlendShape;
    private int currentMesh;

    private bool currentlyUpgrading;
    private int awaitingUpgrades;


    public int collectedPartsCount => collectedParts.Count;

    private void Update()
    {
        partCount = collectedPartsCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(gameObject.tag))
        {
            collectedParts.Push(other.gameObject.transform.parent.parent.gameObject);

            inBetween++;
            CheckCarUpgrade();
        }
    }

    private void CheckCarUpgrade()
    {
        if(inBetween == amountForUpgrade)
        {
            inBetween = 0;

            if(currentBlendShape < carMeshes[currentMesh].sharedMesh.blendShapeCount)
            {
                if (!currentlyUpgrading)
                {
                    StartCoroutine(nameof(UpgradeSmooth));
                    currentlyUpgrading = true;
                }
                else
                {
                    awaitingUpgrades++;
                }
                
            }
            else
            {
                UpgradeCar();
            }          
        }
    }

    public void UpgradeCar()
    {
        if(currentMesh + 1 >= carMeshes.Count) { return; }
        ResetBlendShapes();

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh++;
        amountForUpgrade += upgradeAmountIncrement;
        carMeshes[currentMesh].gameObject.SetActive(true);
    }

    public void DowngradeCar()
    {
        StopAllCoroutines();
        ResetBlendShapes();
        
        amountForUpgrade = Mathf.Clamp(amountForUpgrade - upgradeAmountIncrement,0,99999);
        
        for (int i = 0; i < inBetween; i++)
        {
            collectedParts.Pop();
        }

        if (collectedParts.Count != 0)
        {
            for (int i = 0; i < amountForUpgrade; i++)
            {
                collectedParts.Pop();
            }
        }
        
        awaitingUpgrades = 0;
        inBetween = 0;

        if (currentMesh - 1 <= carMeshes.Count) { return; }
        

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh--;
        carMeshes[currentMesh].gameObject.SetActive(true);
    }

    private void ResetBlendShapes()
    {
        for(int i=0; i< carMeshes[currentMesh].sharedMesh.blendShapeCount; i++)
        {
            carMeshes[currentMesh].SetBlendShapeWeight(i, 100f);
            currentBlendShape = 0;
        }
    }

    private IEnumerator UpgradeSmooth()
    {
        var value = 100f;
        while (value > 0f)
        {
            var clampValue = Mathf.Clamp( value -= 1f - blendSmoothSpeed,0f,100f);
            carMeshes[currentMesh].SetBlendShapeWeight(currentBlendShape, clampValue);
            yield return null;
        }
        
        currentBlendShape++;
        
        if (awaitingUpgrades > 0 && currentBlendShape< carMeshes[0].sharedMesh.blendShapeCount)
        {
            awaitingUpgrades--;
            StartCoroutine(nameof(UpgradeSmooth));
            yield break;
        }
        
        currentlyUpgrading = false;
        yield break;
    }
}
