using UnityEngine;
using UnityEngine.UI;

public class PlayArea : MonoBehaviour
{
    void Start()
    {
        LevelManager.gameState = GameState.BeforeStart;
    }

    public void GameStarter()
    {
        LevelManager.gameState = GameState.Normal;
        UpperPanel.instance.transform.GetChild(0).gameObject.SetActive(true);
        UpperPanel.instance.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
