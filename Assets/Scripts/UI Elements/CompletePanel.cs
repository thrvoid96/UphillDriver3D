using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using ElephantSDK;
using System.Collections;
public class CompletePanel : MonoBehaviour
{
    public static CompletePanel instance;
    private void Awake()
    {
        instance = this;
    }
    public void CompleteButtonsHandle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Activator(bool status = true)
    {
        if (status)
        {
            //Elephant.LevelCompleted(GameManager.Level);
            //LevelManager.gameState = GameState.Victory;
            GameManager.Level++;
            StartCoroutine(Delay());   
        }
        else
        {
            //Elephant.LevelFailed(GameManager.Level);
            //LevelManager.gameState = GameState.Failed;
            StartCoroutine(Delay());
        }
        IEnumerator Delay()
        {
            if (status)
            {
                for (int i = 0; i < 2; i++)
                {
                    Camera.main.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
                }
            }
            yield return new WaitForSeconds(1f);
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            if (status)
            {
                transform.GetChild(2).gameObject.SetActive(true);
                transform.GetChild(1).GetComponent<Text>().text = "VICTORY!";

            }
            else
            {
                transform.GetChild(3).gameObject.SetActive(true);
                transform.GetChild(1).GetComponent<Text>().text = "FAILED!";
            }
        }
    }
}
