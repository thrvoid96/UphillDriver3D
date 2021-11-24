using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Behaviours;
using UnityEngine;

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

public class LevelHolder : MonoBehaviour
{
    public List<GameColor> colorsInThisLevelList = new List<GameColor>();
    public int howManyEnemy;
    public int howManyRequired;
    public List<Ramp> rampsOnScene = new List<Ramp>();

    #region Singleton
    public static LevelHolder instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    
    
    private void Start()
    {
        StructurePanel.instance.CreateLayoutElement(howManyRequired);
        DetermineColorsInThisLevelList();
    }
    
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
        LevelManager.instance.playersOnGameList.Add(PlayerController.instance.gameObject);
        for (int i = 1; i <= howManyEnemy; i++)
        {
            GameObject enemy = LevelManager.instance.GiveEnemyFromGameColor(colorsInThisLevelList[i] , i);
            LevelManager.instance.playersOnGameList.Add(enemy);
        }      
    }

}