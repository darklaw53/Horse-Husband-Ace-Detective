using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StringToUnityEvent
{
    public string Key;
    public UnityEvent Value = new UnityEvent();
}

public class DialogueActionManager : MonoBehaviour
{
    public List<StringToUnityEvent> myEvents = new List<StringToUnityEvent>();

    public void ExecuteAction(string key)
    {
        UnityEvent action = GetActionFromKey(key);
        if (action != null)
        {
            action.Invoke(); 
        }
        else
        {
            Debug.LogWarning($"No action found for key: {key}");
        }
    }

    UnityEvent GetActionFromKey(string key)
    {
        foreach (StringToUnityEvent e in myEvents)
        {
            if (e.Key == key)
                return e.Value;
        }

        return null;
    }
}