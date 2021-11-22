using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPartCollector : MonoBehaviour
{
    [SerializeField] private List<SkinnedMeshRenderer> carMeshes = new List<SkinnedMeshRenderer>();
    [SerializeField] private int amountForUpgrade;
    [SerializeField] private List<GameObject> collectedParts = new List<GameObject>();

    private int inBetween;
    private int currentUpgrade;
    private int currentMesh;


    public int collectedPartsCount
    {
        get { return collectedParts.Count; }
    }

    private void OnTriggerEnter(Collider other)
    {
        collectedParts.Add(other.gameObject.transform.parent.parent.gameObject);

        inBetween++;
        checkCarUpgrade();
    }

    private void checkCarUpgrade()
    {
        if(inBetween == amountForUpgrade)
        {
            inBetween = 0;

            if(currentUpgrade< carMeshes[0].sharedMesh.blendShapeCount)
            {
                carMeshes[currentMesh].SetBlendShapeWeight(currentUpgrade, 0f);
                currentUpgrade++;
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
        if (currentMesh - 1 <= carMeshes.Count) { return; } //Do lose case here.
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
}
