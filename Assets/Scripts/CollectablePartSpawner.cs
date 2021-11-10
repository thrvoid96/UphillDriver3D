using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////Script attached to Grid gameobject. Spawns a grid to be used.

public class CollectablePartSpawner : MonoBehaviour
{
    #region singleton
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

    public List<Grid> grids;

    private void Start()
    {
        for(int i=0; i< grids.Count; i++)
        {
            randomizeSpawnPoints(grids[i]);
            setBlockSpawnPositions(grids[i]);
            spawnAllBlocks(i);
        }
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

    private void setBlockSpawnPositions(Grid grid)
    {
        var list = new List<Vector3>();

        for (int i = 0; i < grid.randomList.Count; i++)
        {
            var x = Mathf.FloorToInt(grid.randomList[i] / grid.gridSizeZ);
            var z = grid.randomList[i] % grid.gridSizeZ;

            Vector3 spawnPosition = new Vector3(x * grid.gridSpacingX, 0, z * grid.gridSpacingZ) + grid.gridOrigin;
            list.Add(spawnPosition);
        }

        grid.blockPositions.Add(0, list);

    }

    private void spawnAllBlocks(int gridIndex)
    {
        for (int i = 0; i < grids[gridIndex].randomList.Count; i++)
        {
            ObjectPooler.instance.SpawnFromPool("CollectablePart", grids[gridIndex].blockPositions[0][i], Quaternion.identity);
        }

    }
}