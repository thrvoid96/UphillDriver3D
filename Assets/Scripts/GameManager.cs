using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region Fields
    static int soundLevel;
    static int vibration;
    static int level;
    static int randomLevel;
    #endregion
    public static int Level
    {
        get
        {
            if (!PlayerPrefs.HasKey("level"))
            {
                return 1;
            }
            return PlayerPrefs.GetInt("level");
        }
        set
        {
            level = value;
            PlayerPrefs.SetInt("level", level);
        }
    }

    public static int RandomLevel
    {
        get
        {
            if (!PlayerPrefs.HasKey("randomLevel"))
            {
                return 1;
            }
            return PlayerPrefs.GetInt("randomLevel");
        }
        set
        {
            randomLevel = value;
            PlayerPrefs.SetInt("randomLevel", randomLevel);
        }
    }
    public static int Vibration
    {
        get
        {
            if (!PlayerPrefs.HasKey("vibration"))
            {
                return 1;
            }
            return PlayerPrefs.GetInt("vibration");
        }
        set
        {
            vibration = value;
            PlayerPrefs.SetInt("vibration", vibration);
        }
    }
    public static int Sound
    {
        get
        {
            return PlayerPrefs.GetInt("soundLevel");
        }
        set
        {
            soundLevel = value;
            PlayerPrefs.SetInt("soundLevel", soundLevel);
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        if (!PlayerPrefs.HasKey("level"))
        {
            PlayerPrefs.SetInt("level", 1);
        }
        if (!PlayerPrefs.HasKey("vibration"))
        {
            PlayerPrefs.SetInt("vibration", 1);
        }
        if (!PlayerPrefs.HasKey("soundLevel"))
        {
            PlayerPrefs.SetInt("soundLevel", 1);
        }
    }
}
