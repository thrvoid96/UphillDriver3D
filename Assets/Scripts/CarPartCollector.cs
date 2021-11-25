using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPartCollector : MonoBehaviour
{
    public int partCount;
    [SerializeField] private List<SkinnedMeshRenderer> carMeshes = new List<SkinnedMeshRenderer>();
    
    [SerializeField] private int amountForUpgrade;
    private int firstAmountForUpgrade;
    
    [SerializeField] private int upgradeAmountIncrement;

    [Header("0 for instant, 1 for slowest")]
    [SerializeField] private float blendSmoothSpeed;
    
    private Stack<GameObject> collectedParts = new Stack<GameObject>();

    public int inBetween,currentBlendShape,currentMesh,awaitingUpgrades;

    public int collectedPartsCount => collectedParts.Count;

    private void Start()
    {
        StartCoroutine(nameof(UpgradeSmooth));
        firstAmountForUpgrade = amountForUpgrade;
    }

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

            if(awaitingUpgrades + currentBlendShape < carMeshes[currentMesh].sharedMesh.blendShapeCount)
            {
                awaitingUpgrades++;
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
        ResetValues();

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh++;
        amountForUpgrade += upgradeAmountIncrement;
        carMeshes[currentMesh].gameObject.SetActive(true);
    }

    public void DowngradeCar()
    {
        var blocksToPopCount = Mathf.Clamp(inBetween + (amountForUpgrade * (currentBlendShape +1)),0,collectedParts.Count);
        
        for (int i = 0; i < blocksToPopCount; i++)
        {
            collectedParts.Pop();
        }
        
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
        awaitingUpgrades = 0;
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
                    currentBlendShape = Mathf.Clamp(currentBlendShape+=1, 0, carMeshes[0].sharedMesh.blendShapeCount);
                    value = 100f; 
                    awaitingUpgrades--;
                    
                    yield return null;
                }
                
            }
            
            yield return null;
        }
    }
}
