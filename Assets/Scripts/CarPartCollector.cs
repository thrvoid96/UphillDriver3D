using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class CarPartCollector : MonoBehaviour
{
    [SerializeField] private List<SkinnedMeshRenderer> carMeshes = new List<SkinnedMeshRenderer>();

    public int collectedPartsCount,inBetween,amountForUpgrade, currentMesh, currentBlendShape;

    private TweenerCore<float, float, FloatOptions> currentTween;
    

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
        inBetween++;
        
        if (inBetween == amountForUpgrade)
        {
            inBetween = 0;

            currentTween?.Complete();
            
            var smoothValue = 100f;

            if (currentBlendShape == carMeshes[currentMesh].sharedMesh.blendShapeCount)
            {
                UpgradeCar();
                return;
            }

            currentTween = DOTween.To(() => smoothValue, x => smoothValue = x, 0f, 2f)
                .OnUpdate(() =>
                {
                    carMeshes[currentMesh].SetBlendShapeWeight(currentBlendShape, smoothValue);
                }).OnComplete(() =>
                {
                    currentBlendShape = Mathf.Clamp(currentBlendShape += 1, 0,
                        carMeshes[currentMesh].sharedMesh.blendShapeCount);
                });
        }
    }

    private void UpgradeCar()
    {
        if(currentMesh == carMeshes.Count - 1) { return; }
        currentBlendShape = 0;

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh++;
        carMeshes[currentMesh].gameObject.SetActive(true);
    }

    public void DowngradeCar()
    {
        currentTween?.Complete();
        
        var blocksToRemove = Mathf.Clamp(inBetween + amountForUpgrade * currentBlendShape + (carMeshes[Mathf.Clamp(currentMesh - 1,0,carMeshes.Count)].sharedMesh.blendShapeCount + 1) * amountForUpgrade, 0,collectedPartsCount);
        
        collectedPartsCount -= blocksToRemove;
        
        inBetween = 0;
        
        currentBlendShape = 0;
        
        ResetShapes();
        
        if (currentMesh == 0) { return; }

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh--;
        carMeshes[currentMesh].gameObject.SetActive(true);
        
        ResetShapes();
    }

    private void ResetShapes()
    {
        for(int i=0; i< carMeshes[currentMesh].sharedMesh.blendShapeCount; i++)
        {
            carMeshes[currentMesh].SetBlendShapeWeight(i, 100f);
        }
    }
    
    
}
