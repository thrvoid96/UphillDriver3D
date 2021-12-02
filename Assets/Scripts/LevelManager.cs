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
    public List<GameObject> floorPrefabList;
    public List<CommonBehaviours> playersOnGameList;

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
        floorPrefabList = levelAsset.floorPrefabs;

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
        
        var carPartCollector = createdEnemy.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<CarPartCollector>();
        
        createdEnemy.transform.GetChild(0).GetChild(0).GetChild(0).tag = "Player" + playerNum;
        createdEnemy.name = color + "_Enemy";

        ColorPlayer(common.color, carPartCollector);
        
        return createdEnemy;
    }

    public void ColorPlayer(GameColor color, CarPartCollector carPartCollector)
    {
        switch (color)
        {
            case GameColor.PlayerColor:
                foreach (var mesh in carPartCollector.carMeshes)
                {
                    mesh.material = materials[0];
                }
                break;

            case GameColor.Red:
                foreach (var mesh in carPartCollector.carMeshes)
                {
                    mesh.material = materials[1];
                }
                break;

            case GameColor.Green:
                foreach (var mesh in carPartCollector.carMeshes)
                {
                    mesh.material = materials[2];
                }
                break;

            case GameColor.Orange:
                foreach (var mesh in carPartCollector.carMeshes)
                {
                    mesh.material = materials[3];
                }
                break;

            case GameColor.Yellow:
                foreach (var mesh in carPartCollector.carMeshes)
                {
                    mesh.material = materials[4];
                }
                break;

            case GameColor.Pink:
                foreach (var mesh in carPartCollector.carMeshes)
                {
                    mesh.material = materials[5];
                }
                break;

            case GameColor.Cyan:
                foreach (var mesh in carPartCollector.carMeshes)
                {
                    mesh.material = materials[6];
                }
                break;

            default:
                Debug.LogError("Switch error");
                break;

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

            case GameColor.Cyan:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[6];
                return objToSpawn;

            default:
                objToSpawn.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[0];
                return objToSpawn;

        }
    }
    

}
