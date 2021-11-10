using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class VictoryPanel : MonoBehaviour
{
    public static VictoryPanel instance;

    private void Awake()
    {
        instance = this;
    }
    public void CompleteButtonsHandle()
    {
        GameManager.Level++;
        GameManager.RandomLevel = 0;
        StartCoroutine(Delay());

    }

    public void CompleteButtonsHandleWithoutAdd()
    {
        GameManager.RandomLevel = 0;
        StartCoroutine(Delay());
    }

    public void VictoryCase()
    {       
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        AudioManager.Play(AudioClipName.Victory);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }



}
