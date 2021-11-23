using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPartCollector : MonoBehaviour
{
    [SerializeField] private List<SkinnedMeshRenderer> carMeshes = new List<SkinnedMeshRenderer>();
    [SerializeField] private int amountForUpgrade;
    [SerializeField] private List<GameObject> collectedParts = new List<GameObject>();
    
    [Header("0 for instant, 1 for slowest")]
    [SerializeField] private float blendSmoothSpeed;

    private int inBetween;
    private int currentUpgrade;
    private int currentMesh;

    private bool currentlyUpgrading;
    private int awaitingUpgrades;


    public int collectedPartsCount => collectedParts.Count;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(gameObject.tag))
        {
            collectedParts.Add(other.gameObject.transform.parent.parent.gameObject);

            inBetween++;
            CheckCarUpgrade();
        }
    }

    private void CheckCarUpgrade()
    {
        if(inBetween == amountForUpgrade)
        {
            inBetween = 0;

            if(currentUpgrade< carMeshes[0].sharedMesh.blendShapeCount)
            {
                if (!currentlyUpgrading)
                {
                    StartCoroutine(nameof(BlendShapeSmooth));
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
        carMeshes[currentMesh].gameObject.SetActive(true);
    }

    public void DowngradeCar()
    {
        if (currentMesh - 1 <= carMeshes.Count) { return; }
        
        StopAllCoroutines();
        ResetBlendShapes();

        carMeshes[currentMesh].gameObject.SetActive(false);
        currentMesh--;
        carMeshes[currentMesh].gameObject.SetActive(true);
    }

    private void ResetBlendShapes()
    {
        for(int i=0; i< carMeshes[0].sharedMesh.blendShapeCount; i++)
        {
            carMeshes[currentMesh].SetBlendShapeWeight(i, 100f);
            currentUpgrade = 0;
        }
    }

    private IEnumerator BlendShapeSmooth()
    {
        var value = 100f;
        while (value > 0f)
        {
            var clampValue = Mathf.Clamp( value -= 1f - blendSmoothSpeed,0f,100f);
            carMeshes[currentMesh].SetBlendShapeWeight(currentUpgrade, clampValue);
            yield return null;
        }
        
        currentUpgrade++;
        
        if (awaitingUpgrades > 0 && currentUpgrade< carMeshes[0].sharedMesh.blendShapeCount)
        {
            awaitingUpgrades--;
            StartCoroutine(nameof(BlendShapeSmooth));
            yield break;
        }
        
        currentlyUpgrading = false;
        yield break;
    }
}
