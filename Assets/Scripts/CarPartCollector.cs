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
    public List<SkinnedMeshRenderer> carMeshes = new List<SkinnedMeshRenderer>();

    public int collectedPartsCount,inBetween,amountForUpgrade, currentMesh, currentBlendShape, speedUpgrade, losePartAmount , rotatingWheelCount, nonRotatingWheelCount;

    public bool losePartsAfterRamp;
    
    [Header("ONLY ADD FOR PLAYER NOT AI")]
    public CameraFollow followCamera;

    private TweenerCore<float, float, FloatOptions> currentTween;

    private CommonBehaviours currentPlayer;

    private void Start()
    {
        currentPlayer = transform.parent.GetComponent<CommonBehaviours>();
        
        ChangeWheelsAndTrails(currentMesh);
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

        currentPlayer.transform.DOScale(transform.parent.localScale + new Vector3(1f,1f,1f), 1f).SetEase(Ease.InOutSine);

        currentPlayer.getMaxSpeed += speedUpgrade;
        
        currentPlayer.ReCalculateSpeedRatio();
        
        ChangeWheelsAndTrails(currentMesh);

        if (currentPlayer.getPlayerNum == 0)
        {
            currentPlayer.RotateWheelsAfterVehicleChange();
        
            followCamera.ChangeCameraZoom(true);
        }
        
    }

    private void DowngradeCar()
    {
        if (currentMesh == 0) { return; }

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh--;
        carMeshes[currentMesh].gameObject.SetActive(true);
        
        currentPlayer.transform.DOScale(transform.parent.localScale + new Vector3(1f,1f,1f), 1f).SetEase(Ease.InOutSine);
        
        currentPlayer.getMaxSpeed -= speedUpgrade;
        
        currentPlayer.ReCalculateSpeedRatio();
        
        ChangeWheelsAndTrails(currentMesh);
        
        if (currentPlayer.getPlayerNum == 0)
        {
            currentPlayer.RotateWheelsAfterVehicleChange();
        
            followCamera.ChangeCameraZoom(false);
        }
    }

    private void ChangeWheelsAndTrails(int index)
    {
        currentPlayer.currentTrails.Clear();
        
        currentPlayer.currentWheels.Clear();

        for (int i = 0; i < rotatingWheelCount + nonRotatingWheelCount; i++)
        {
            currentPlayer.currentWheels.Add(transform.parent.GetChild(index + 1).GetChild(i).gameObject);
        }

        for (int i = 0; i < nonRotatingWheelCount; i++)
        {
            currentPlayer.currentTrails.Add(transform.parent.GetChild(index + 1).GetChild(i+2).GetChild(0).GetComponent<TrailRenderer>());
        }
    }
    
    public void CalculatePartsAfterRamp(int amount)
    {
        if (losePartsAfterRamp)
        {
            var blocksToRemove = Mathf.Clamp(inBetween + amount, 0,collectedPartsCount);
        
            collectedPartsCount -= blocksToRemove;

            var startBlendShape = currentBlendShape;

            currentBlendShape -= amount/amountForUpgrade;

            if (currentBlendShape >= 0)
            {
                //Lose parts from current mesh
                LoseParts(currentBlendShape, currentBlendShape + amount/amountForUpgrade);
            }
            else
            {
                //Lose parts from both meshes
                LoseParts(0, startBlendShape);

                if (currentMesh != 0)
                {
                    DowngradeCar();
            
                    currentBlendShape = (carMeshes[currentMesh].sharedMesh.blendShapeCount) - (amount/amountForUpgrade - startBlendShape) + 1;
            
                    LoseParts(currentBlendShape, carMeshes[currentMesh].sharedMesh.blendShapeCount);
                }
                else
                {
                    currentBlendShape = 0;
                }

                //Lose parts from last mesh
            }
        }
        
    }

    public void CalculatePartsAfterCollision()
    {
        currentTween?.Complete();
        
        var blocksToRemove = Mathf.Clamp(inBetween + amountForUpgrade * losePartAmount, 0,collectedPartsCount);
        
        collectedPartsCount -= blocksToRemove;

        var startBlendShape = currentBlendShape;

        currentBlendShape -= losePartAmount;

        if (currentBlendShape >= 0)
        {
            //Lose parts from current mesh
            LoseParts(currentBlendShape, currentBlendShape + losePartAmount);
        }
        else
        {
            //Lose parts from both meshes
            LoseParts(0, startBlendShape);

            if (currentMesh != 0)
            {
                DowngradeCar();
            
                currentBlendShape = (carMeshes[currentMesh].sharedMesh.blendShapeCount) - (losePartAmount - startBlendShape) + 1;
            
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
        for(int i = startIndex; i < endIndex; i++)
        {
            carMeshes[currentMesh].SetBlendShapeWeight(i, 100f);
        }
        
        inBetween = 0;
    }

}
