using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LosePanel : MonoBehaviour
{
    public static LosePanel instance;
    Button button;

    private void Awake()
    {
        instance = this;
        button = transform.GetChild(2).GetComponent<Button>();
    }
    public void CompleteButtonsHandle()
    {
        StartCoroutine(DelayForBuild2());
        button.interactable = false;
    }

    public void LoseCase()
    {
        StartCoroutine(DelayForPanel());
    }

    public void LoseCaseWithoutDelay()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        AudioManager.Play(AudioClipName.Failed);
    }

    IEnumerator DelayForPanel()
    {
        yield return new WaitForSeconds(1f);
        LoseCaseWithoutDelay();
    }


    IEnumerator DelayForBuild2()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
