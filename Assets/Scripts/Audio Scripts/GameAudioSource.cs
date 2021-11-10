using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An audio source for the entire game
/// </summary>
public class GameAudioSource : MonoBehaviour
{

    #region Fields
    static GameAudioSource instance;
    [HideInInspector] public AudioSource audioSource;
 
    #endregion

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // initialize audio manager for PlayOneShot
            audioSource = gameObject.AddComponent<AudioSource>();
            AudioManager.Initialize(audioSource);
        }
    }
}
