using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Events
{
    LevelWon,
    LevelLost
}

public class EventManager : MonoBehaviour
{
    private Dictionary<Events, Action<EventParam>> eventDictionary;

    private static EventManager eventManager;

    private static EventParam param = new EventParam();

    public static EventParam nullEventParam => param;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }

        set { }
    }

    private void OnDisable()
    {
        instance = null;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<Events, Action<EventParam>>();
        }
    }

    public static void StartListening(Events eventName, Action<EventParam> listener)
    {
        if (instance.eventDictionary.ContainsKey(eventName))
        {
            instance.eventDictionary[eventName] += listener;
        }
        else
        {
            instance.eventDictionary.Add(eventName, listener);
        }
    }

    public static void StopListening(Events eventName, Action<EventParam> listener)
    {
        if (instance.eventDictionary.ContainsKey(eventName))
        {
            instance.eventDictionary[eventName] -= listener;
        }
    }

    public static void TriggerEvent(Events eventName, EventParam eventParam)
    {
        //Debug.LogError(eventName);
        Action<EventParam> thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(eventParam);
        }
    }
}

//Re-usable structure/ Can be a class to. Add all parameters you need inside it
public struct EventParam
{
    public bool levelWon;
}