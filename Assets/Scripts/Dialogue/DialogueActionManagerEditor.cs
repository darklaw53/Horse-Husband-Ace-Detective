#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueActionManager))]
[CanEditMultipleObjects] 
public class DialogueActionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Save Keys To ScriptableObject"))
        {
            foreach (Object t in targets) 
            {
                DialogueActionManager manager = t as DialogueActionManager;
                if (manager != null)
                {
                    manager.SaveKeysToAsset();
                }
            }
        }
    }
}
#endif