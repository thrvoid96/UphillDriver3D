using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LevelAsset", menuName = "ScriptableObjects/LevelAssetCreate", order = 1)]
public class LevelAssetCreate : ScriptableObject
{
    public List<GameObject> levelPrefabs;
    public GameObject layoutElements;
    public Sprite[] layoutElementComponents;
}
