using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

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

    public void SaveKeysToAsset()
    {
        DialogueActionKeys keysAsset = DialogueActionKeys.Instance;
        if (keysAsset == null)
        {
            Debug.LogError("Could not find DialogueActionKeys asset!");
            return;
        }

        keysAsset.keys.Clear();

        foreach (StringToUnityEvent e in myEvents)
        {
            if (!string.IsNullOrEmpty(e.Key) && !keysAsset.keys.Contains(e.Key))
                keysAsset.keys.Add(e.Key);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(keysAsset);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Saved {keysAsset.keys.Count} keys to {keysAsset.name} asset!");
    }
}