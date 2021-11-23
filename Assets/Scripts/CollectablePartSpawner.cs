using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////Script attached to Grid gameobject. Spawns a grid to be used.

public class CollectablePartSpawner : MonoBehaviour
{
    #region Singleton
    public static CollectablePartSpawner instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion
    
    [System.Serializable]
    public class Grid
    {
        public int gridSizeX;
        public int gridSizeZ;
        public float gridSpacingX = 1f;
        public float gridSpacingZ = 1f;
        public Vector3 gridOrigin = Vector3.zero;
        [System.NonSerialized] public List<int> randomList = new List<int>();
        [System.NonSerialized] public Dictionary<int, List<Vector3>> blockPositions = new Dictionary<int, List<Vector3>>();
    }

    [SerializeField] private int playerCount = 5;
    public List<Grid> grids;

    private void Start()
    {
        foreach (Grid grid in grids)
        {
            randomizeSpawnPoints(grid);

            for (int i = 0; i < playerCount; i++)
            {
                setBlockSpawnPositions(grid, i);
            }
        }

        for (int i = 0; i < playerCount; i++)
        {
            SpawnAllPartsForPlayer(i, 0);
        }
    }

    public List<Vector3> getBlockPositionsForPlayer(int gridIdx, int playerNumber)
    {
        return grids[gridIdx].blockPositions[playerNumber];
    }


    private void randomizeSpawnPoints(Grid grid)
    {

        List<int> uniqueNumbers = new List<int>();

        for (int i = 0; i < grid.gridSizeX * grid.gridSizeZ; i++)
        {
            uniqueNumbers.Add(i);
        }
        for (int i = 0; i < grid.gridSizeX * grid.gridSizeZ; i++)
        {
            int ranNum = uniqueNumbers[Random.Range(0, uniqueNumbers.Count)];
            grid.randomList.Add(ranNum);
            uniqueNumbers.Remove(ranNum);
        }
    }

    private void setBlockSpawnPositions(Grid grid, int playerNum)
    {
        var longNum = grid.randomList.Count / playerCount;
        var list = new List<Vector3>();

        for (int i = playerNum * longNum; i < (playerNum + 1) * longNum; i++)
        {
            var x = Mathf.FloorToInt(grid.randomList[i] / grid.gridSizeZ);
            var z = grid.randomList[i] % grid.gridSizeZ;

            Vector3 spawnPosition = new Vector3(x * grid.gridSpacingX, 0, z * grid.gridSpacingZ) + grid.gridOrigin;
            list.Add(spawnPosition);
        }

        grid.blockPositions.Add(playerNum, list);

    }

    public void SpawnAllPartsForPlayer(int playerNum, int gridIndex)
    {
        var longNum = grids[gridIndex].randomList.Count / playerCount;

        for (int i = 0; i < longNum; i++)
        {
            ObjectPooler.instance.SpawnFromPool("Player" + playerNum, grids[gridIndex].blockPositions[playerNum][i], Quaternion.identity);
        }

    }
}