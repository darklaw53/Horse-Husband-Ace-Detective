#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueActionKeys", menuName = "ScriptableObjects/DialogueActionKeys", order = 1)]
public class DialogueActionKeys : ScriptableObject
{
    private static DialogueActionKeys _instance;

    public static DialogueActionKeys Instance
    {
        get
        {
            if (_instance == null)
            {
#if UNITY_EDITOR
                string[] guids = AssetDatabase.FindAssets("t:DialogueActionKeys");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    _instance = AssetDatabase.LoadAssetAtPath<DialogueActionKeys>(path);
                }
#endif
            }
            return _instance;
        }
    }

    [SerializeField]
    public List<string> keys = new List<string>();
}