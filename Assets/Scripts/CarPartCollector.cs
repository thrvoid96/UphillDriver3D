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
        collectedParts.Add(other.gameObject.transform.parent.parent.gameObject);
    }
}
