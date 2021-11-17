using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    #region Singleton
    public static SceneSetup instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public List<Floors> floorsOnScene = new List<Floors>();


    [System.Serializable]
    public class Floors
    {
        public List<int> gridIndexes = new List<int>();
        public List<Transform> AIPositionsToGo = new List<Transform>();
    }

    

}
