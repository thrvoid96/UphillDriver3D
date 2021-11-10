using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using ElephantSDK;
public class UpperPanel : MonoBehaviour
{
    public static UpperPanel instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        transform.GetChild(2).GetComponent<Text>().text = "LEVEL "+GameManager.Level + string.Empty;
    }

    public void Activater(bool status = true, int childIndex = int.MinValue)
    {
        if (childIndex != int.MinValue)
        {
            transform.GetChild(childIndex).gameObject.SetActive(status);
        }
    }

    public void RetryButtonHandleEvent()
    {
        //Elephant.LevelFailed(GameManager.Level);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
