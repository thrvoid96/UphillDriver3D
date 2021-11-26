using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Behaviours;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameColor
{
    Red,
    PlayerColor,
    Green,
    Orange,
    Yellow,
    Pink,
    Black,
}

public enum RampType
{
    LowRamp,
    MidRamp,
    HighRamp
}

public class LevelHolder : MonoBehaviour
{
    public List<GameColor> colorsInThisLevelList = new List<GameColor>();
    public int howManyEnemy;
    LevelAssetCreate levelAsset;
    
    [System.Serializable]
    public class Floor
    {
        [Header("Ramp Stats")]
        public RampType rampType;
        public int blocksToPassRamp;
        
        [Header("AI Stats")]
        public List<Transform> AIPositionsToGo = new List<Transform>();
        
        [Header("Grid Stats")]
        public int gridSizeX = 10;
        public int gridSizeZ = 10;
        public float gridSpacingX = 6f;
        public float gridSpacingZ = 6f;
        public Transform gridOrigin;
        [System.NonSerialized] public int directionX,directionZ;
        [System.NonSerialized] public List<int> randomList = new List<int>();
        [System.NonSerialized] public Dictionary<int, List<Vector3>> blockPositions = new Dictionary<int, List<Vector3>>();
    }
    
    public List<Floor> howManyFloors;

    #region Singleton
    public static LevelHolder instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    
    
    private void Start()
    {
        var howMany = 0;
        StructurePanel.instance.CreateLayoutElement(howMany);
        DetermineColorsInThisLevelList();
        
        //CreateFloors();
        CreateGridPositions();
    }
    
    private void CreateGridPositions()
    {
        foreach (Floor floor in howManyFloors)
        {
            CalculateGridOriginDirection(floor);

            randomizeSpawnPoints(floor);

            for (int i = 0; i < LevelManager.instance.playersOnGameList.Count; i++)
            {
                setBlockSpawnPositions(floor, i);
            }

            AddMissingBlocks(floor);
        }
        

        for (int i = 0; i < LevelManager.instance.playersOnGameList.Count; i++)
        {
            SpawnAllPartsForPlayer(i, 0);
        }
    }
    
    private void randomizeSpawnPoints(Floor floor)
    {

        var uniqueNumbers = new List<int>();

        for (int i = 0; i < floor.gridSizeX * floor.gridSizeZ; i++)
        {
            uniqueNumbers.Add(i);
        }
        for (int i = 0; i < floor.gridSizeX * floor.gridSizeZ; i++)
        {
            int ranNum = uniqueNumbers[Random.Range(0, uniqueNumbers.Count)];
            floor.randomList.Add(ranNum);
            uniqueNumbers.Remove(ranNum);
        }
    }

    private void setBlockSpawnPositions(Floor floor, int playerNum)
    {
        var longNum = floor.randomList.Count / LevelManager.instance.playersOnGameList.Count;
        var list = new List<Vector3>();

        for (int i = playerNum * longNum; i < (playerNum + 1) * longNum; i++)
        {
            var x = Mathf.FloorToInt(floor.randomList[i] / floor.gridSizeZ);
            var z = floor.randomList[i] % floor.gridSizeZ;

            Vector3 spawnPosition = new Vector3(x * floor.gridSpacingX, 0, z * floor.gridSpacingZ) 
                                    + floor.gridOrigin.transform.position;
            
            list.Add(spawnPosition);
        }
        
        floor.blockPositions.Add(playerNum , list);
    }
    
    private void AddMissingBlocks(Floor floor)
    {
        var difference = floor.randomList.Count % LevelManager.instance.playersOnGameList.Count;
            
        if (difference > 0)
        {
            for (int i = floor.randomList.Count - difference; i < floor.randomList.Count; i++)
            {
                var x = Mathf.FloorToInt(floor.randomList[i] / floor.gridSizeZ);
                var z = floor.randomList[i] % floor.gridSizeZ;

                Vector3 spawnPosition = new Vector3(x * floor.gridSpacingX, 0, z * floor.gridSpacingZ)
                                        + floor.gridOrigin.transform.position;

                var randomPlayer = Random.Range(0, LevelManager.instance.playersOnGameList.Count);
                floor.blockPositions[randomPlayer].Add(spawnPosition);
            }
        }
    }
    
    private void CalculateGridOriginDirection(Floor floor)
    {
        if (floor.gridSizeX > 10)
        {
            floor.directionX = -1;
        }
        else if (floor.gridSizeX < 10)
        {
            floor.directionX = 1;
        }
        else
        {
            floor.directionX = 0;
        }
            
        if (floor.gridSizeZ > 10)
        {
            floor.directionZ = -1;
        }
        else if (floor.gridSizeZ<10)
        {
            floor.directionZ = 1;
        }
        else
        {
            floor.directionZ = 0;
        }
    }
    
    public List<Vector3> getBlockPositionsForPlayer(int gridIdx, int playerNumber)
    {
        return howManyFloors[gridIdx].blockPositions[playerNumber];
    }
    
    public void SpawnAllPartsForPlayer(int playerNum, int gridIndex)
    {
        for (int i = 0; i < howManyFloors[gridIndex].blockPositions[playerNum].Count; i++)
        {
            var objToSpawn= ObjectPooler.instance.SpawnFromPool("Part", howManyFloors[gridIndex].blockPositions[playerNum][i], Quaternion.identity,howManyFloors[gridIndex].gridOrigin ,playerNum);
        }

    }


    
    //----------------------------------------------------------------------------------------//
    /*private void CreateFloors()
    {
        for (int i = 0; i < howManyFloors.Count + 1; i++)
        {
            switch (howManyFloors[i].rampType)
            {
                case RampType.LowRamp:
                    LevelAssetCreate.insta
                    createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[0];
                    return createdEnemy;

                case RampType.MidRamp:
                    createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[1];
                    return createdEnemy;

                case RampType.HighRamp:
                    createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material = materials[2];
                    return createdEnemy;
                default:
                    createdEnemy.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material = materials[0];
                    return createdEnemy;

            }
        }
        
    }*/
    
    //----------------------------------------------------------------------------------------//
    void DetermineColorsInThisLevelList()
    {
        howManyEnemy = (howManyEnemy < LevelManager.instance.gameColor.Count - 1) ? howManyEnemy : LevelManager.instance.gameColor.Count - 1;

        var temp = new List<GameColor>();
        temp.AddRange(LevelManager.instance.gameColor);
        temp.RemoveAt(0);

        for (int i = 0; i < howManyEnemy; i++)
        {
            int rand = Random.Range(0,temp.Count);
            colorsInThisLevelList.Add(temp[rand]);
            temp.RemoveAt(rand);
            
        }

        colorsInThisLevelList.Insert(0, GameColor.PlayerColor);
        CreateEnemies();
    }

    //----------------------------------------------------------------------------------------//
    public void CreateEnemies()
    {
        LevelManager.instance.playersOnGameList.Add(PlayerController.instance.GetComponent<CommonBehaviours>());
        for (int i = 1; i <= howManyEnemy; i++)
        {
            GameObject enemy = LevelManager.instance.GiveEnemyFromGameColor(colorsInThisLevelList[i] , i);
            LevelManager.instance.playersOnGameList.Add(enemy.GetComponent<CommonBehaviours>());
        }      
    }

}