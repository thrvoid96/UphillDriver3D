using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHolder : MonoBehaviour
{
    public int howManyRequired;


    private void Start()
    {
        StructurePanel.instance.CreateLayoutElement(howManyRequired);
    }

}
