using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CarPartCollector : MonoBehaviour
{
    [SerializeField] private List<SkinnedMeshRenderer> carMeshes = new List<SkinnedMeshRenderer>();
    
    [SerializeField] private int amountForUpgrade;
    private int firstAmountForUpgrade;
    
    [SerializeField] private int upgradeAmountIncrement;

    [Header("0 for instant, 1 for slowest")]
    [SerializeField] private float blendSmoothSpeed;
    
    public int collectedPartsCount;

    public int inBetween,currentBlendShape,currentMesh,awaitingUpgrades,totalAmountNeeded;

    private List<int> totalNeededForEachCar = new List<int>();



    private void Start()
    {
        StartCoroutine(nameof(UpgradeSmooth));
        firstAmountForUpgrade = amountForUpgrade;
        
        for (int i = 0; i < carMeshes.Count; i++)
        {
            totalAmountNeeded += (amountForUpgrade + (upgradeAmountIncrement * i)) *
                                 carMeshes[i].sharedMesh.blendShapeCount;
            totalNeededForEachCar.Add((amountForUpgrade + (upgradeAmountIncrement * i)) * carMeshes[i].sharedMesh.blendShapeCount);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(gameObject.tag))
        {
            collectedPartsCount++;

            CheckCarUpgrade();
        }
    }

    private void CheckCarUpgrade()
    {
        if (collectedPartsCount <= totalAmountNeeded)
        {
            inBetween++;
            
            if (inBetween == amountForUpgrade)
            {
                inBetween = 0;
                
                awaitingUpgrades++;
                
                if (collectedPartsCount == totalNeededForEachCar[currentMesh])
                {
                    amountForUpgrade += upgradeAmountIncrement;
                }
            }
        }
    }

    public void UpgradeCar()
    {
        if(currentMesh + 1 >= carMeshes.Count) { return; }
        ResetValues();

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh++;
        carMeshes[currentMesh].gameObject.SetActive(true);
    }

    public void DowngradeCar()
    {
        var blocksToPopCount = Mathf.Clamp(inBetween + (amountForUpgrade * (currentBlendShape +1)),0,collectedPartsCount);

        collectedPartsCount -= blocksToPopCount;

        awaitingUpgrades = 0;
        
        ResetValues();
        
        amountForUpgrade = Mathf.Clamp(amountForUpgrade - upgradeAmountIncrement,firstAmountForUpgrade,99999);

        if (currentMesh - 1 <= carMeshes.Count) { return; }
        

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh--;
        carMeshes[currentMesh].gameObject.SetActive(true);
    }

    private void ResetValues()
    {
        for(int i=0; i< carMeshes[currentMesh].sharedMesh.blendShapeCount; i++)
        {
            carMeshes[currentMesh].SetBlendShapeWeight(i, 100f);
        }
        currentBlendShape = 0;
        inBetween = 0;
    }

    private IEnumerator UpgradeSmooth()
    {
        var value = 100f;
        while (true)
        {
            if (awaitingUpgrades > 0)
            {
                if (value > 0f)
                {
                    var clampValue = Mathf.Clamp(value -= 1f - blendSmoothSpeed, 0f, 100f);
                    carMeshes[currentMesh].SetBlendShapeWeight(currentBlendShape, clampValue);
                    yield return null;
                }
                else
                {
                    currentBlendShape = Mathf.Clamp(currentBlendShape+=1, 0, carMeshes[currentMesh].sharedMesh.blendShapeCount);
                    value = 100f; 
                    awaitingUpgrades--;
                    
                    if (currentBlendShape == carMeshes[0].sharedMesh.blendShapeCount)
                    {
                        UpgradeCar();
                    }
                    
                    yield return null;
                }
                
                
            }
            
            yield return null;
        }
    }
}
