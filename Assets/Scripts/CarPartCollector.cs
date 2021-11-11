using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPartCollector : MonoBehaviour
{
    [SerializeField] private List<GameObject> collectedParts = new List<GameObject>();


    public int collectedPartsCount
    {
        get { return collectedParts.Count; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CollectablePart"))
        {
            collectedParts.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
}
