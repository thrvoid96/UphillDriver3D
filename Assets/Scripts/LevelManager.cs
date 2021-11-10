using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static GameState gameState;

    [System.NonSerialized] public int propertyId;
    LevelAssetCreate levelAsset;
    public bool isLevelCompletedSuccesfully = false;
    private void Awake()
    {
        instance = this;
       
    }
    //-------------------------------------------------------------------------------------------
    private void Start()
    {
        isLevelCompletedSuccesfully = false;
        propertyId = Shader.PropertyToID("_Cutoff");
        levelAsset = Resources.Load<LevelAssetCreate>("Scriptables/LevelAsset");

        Levelgenerator();
    }

    //-------------------------------------------------------------------------------------------
    public void Levelgenerator()
    {
        if (GameManager.Level <= levelAsset.levelPrefabs.Count)
        {
            GameObject createdLevel = Instantiate(levelAsset.levelPrefabs[GameManager.Level - 1]) as GameObject;
        }
        else
        {
            int random = UnityEngine.Random.Range(1, levelAsset.levelPrefabs.Count);
            if (GameManager.RandomLevel == 0)
            {
                GameManager.RandomLevel = random;
            }
            GameObject createdLevel = Instantiate(levelAsset.levelPrefabs[GameManager.RandomLevel]) as GameObject;
        }
    }
    //-------------------------------------------------------------------------------------------
    public void Victory()
    {
        VictoryPanel.instance.VictoryCase();
        Debug.Log("VICTORY");
    }

    //-------------------------------------------------------------------------------------------
    public void Fail()
    {
        LosePanel.instance.LoseCase();
        Debug.Log("FAILED");
    }

}
