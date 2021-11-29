using System;
using System.Collections;
using System.Collections.Generic;
using Behaviours;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class CarPartCollector : MonoBehaviour
{
    [SerializeField] private List<SkinnedMeshRenderer> carMeshes = new List<SkinnedMeshRenderer>();

    public int collectedPartsCount,inBetween,amountForUpgrade, currentMesh, currentBlendShape, speedUpgrade, loseMultiplier;

    private TweenerCore<float, float, FloatOptions> currentTween;

    private CommonBehaviours currentPlayer;

    private void Start()
    {
        currentPlayer = transform.parent.GetComponent<CommonBehaviours>();
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

        inBetween = 0;
        currentBlendShape = 0;
        
        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh++;
        carMeshes[currentMesh].gameObject.SetActive(true);

        currentPlayer.transform.DOScale(transform.parent.localScale * 1.5f, 1f).SetEase(Ease.InOutSine);

        currentPlayer.getMaxSpeed += speedUpgrade;
        
        currentPlayer.ReCalculateSpeedRatio();
    }

    private void DowngradeCar()
    {
        if (currentMesh == 0) { return; }

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh--;
        carMeshes[currentMesh].gameObject.SetActive(true);
        
        currentPlayer.transform.DOScale(transform.parent.localScale / 1.5f, 1f).SetEase(Ease.InOutSine);
        
        currentPlayer.getMaxSpeed -= speedUpgrade;
        
        currentPlayer.ReCalculateSpeedRatio();
    }

    public void CalculatePartsAfterCollision()
    {
        currentTween?.Complete();
        
        var blocksToRemove = Mathf.Clamp(inBetween + amountForUpgrade * loseMultiplier, 0,collectedPartsCount);
        
        collectedPartsCount -= blocksToRemove;

        var startBlendShape = currentBlendShape;

        currentBlendShape -= loseMultiplier;
        
        Debug.LogError(currentBlendShape);

        if (currentBlendShape >= 0)
        {
            //Lose parts from current mesh
            LoseParts(currentBlendShape, currentBlendShape + loseMultiplier);
        }
        else
        {
            //Lose parts from both meshes
            LoseParts(0, startBlendShape);

            if (currentMesh != 0)
            {
                DowngradeCar();
            
                currentBlendShape = (carMeshes[currentMesh].sharedMesh.blendShapeCount) - (loseMultiplier - startBlendShape) + 1;
            
                LoseParts(currentBlendShape, carMeshes[currentMesh].sharedMesh.blendShapeCount);
            }
            else
            {
                currentBlendShape = 0;
            }

            //Lose parts from last mesh
        }
    }
    private void LoseParts(int startIndex, int endIndex)
    {
        Debug.LogError(startIndex);
        Debug.LogError(endIndex);
        for(int i = startIndex; i < endIndex; i++)
        {
            carMeshes[currentMesh].SetBlendShapeWeight(i, 100f);
        }
        
        inBetween = 0;
    }
    
    
}
