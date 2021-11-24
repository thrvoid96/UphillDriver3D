using System.Collections;
using System.Collections.Generic;
using Behaviours;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static GameState gameState;
    
    [HideInInspector] public GameObject enemy_Prefab;
    
    public List<GameColor> gameColor;
    public List<Material> materials;
    public List<GameObject> playersOnGameList;
    
    LevelAssetCreate levelAsset;

    private int loopAmount;
    private int i = 1;

    private void Awake()
    {
        instance = this;
    }
    //-------------------------------------------------------------------------------------------
    private void Start()
    {
        levelAsset = Resources.Load<LevelAssetCreate>("Scriptables/LevelAsset");
        enemy_Prefab = levelAsset.enemyPrefab;
        gameColor = levelAsset.gameColor;
        materials = levelAsset.materials;

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
    
    
    //--------------------------------------------------------------------------------------------------------------//
    public GameObject GiveEnemyFromGameColor(GameColor color, int playerNum)
    {
        var pos= PlayerController.instance.gameObject.transform.position + new Vector3(15f * i,0,0);
        i = -i;
        loopAmount++;
        if (loopAmount % 2 == 0)
        {
            i += i;
        }
        
        var createdEnemy = Instantiate(enemy_Prefab.gameObject , pos , Quaternion.identity);
        
        var common = createdEnemy.transform.GetChild(0).GetChild(0).GetComponent<CommonBehaviours>();
        common.color = color;
        common.getPlayerNum = playerNum;
        
        createdEnemy.transform.GetChild(0).GetChild(0).GetChild(0).tag = "Player" + playerNum;
        createdEnemy.name = color + "_Enemy";
        


        switch (color)
        {
            case GameColor.PlayerColor:
                createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[0];
                return createdEnemy;

            case GameColor.Red:
                createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[1];
                return createdEnemy;

            case GameColor.Green:
                createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[2];
                return createdEnemy;

            case GameColor.Orange:
                createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[3];
                return createdEnemy;

            case GameColor.Yellow:
                createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[4];
                return createdEnemy;

            case GameColor.Pink:
                createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[5];
                return createdEnemy;

            case GameColor.Black:
                createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[6];
                return createdEnemy;

            default:
                createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = materials[0];
                return createdEnemy;

        }
    }
    
    //--------------------------------------------------------------------------------------------------------------//
    public GameObject GiveBrickPrefabFromGameColor(GameColor color, GameObject objToSpawn, int playerNum)
    {
        objToSpawn.tag = "Player" + playerNum;
        objToSpawn.transform.GetChild(0).GetChild(0).tag = "Player" + playerNum;
        
        switch (color)
        {
            case GameColor.PlayerColor:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[0];
                return objToSpawn;

            case GameColor.Red:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[1];
                return objToSpawn;

            case GameColor.Green:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[2];
                return objToSpawn;

            case GameColor.Orange:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[3];
                return objToSpawn;

            case GameColor.Yellow:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[4];
                return objToSpawn;

            case GameColor.Pink:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[5];
                return objToSpawn;

            case GameColor.Black:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[6];
                return objToSpawn;

            default:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[0];
                return objToSpawn;

        }
    }
    

}
