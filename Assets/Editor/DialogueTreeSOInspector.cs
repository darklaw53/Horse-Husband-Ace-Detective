using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueTreeSO))]
public class DialogueTreeSOInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Open in Dialogue Editor"))
        {
            DialogueTreeSO tree = (DialogueTreeSO)target;
            DialogueTreeEditor.OpenWithAsset(tree);
        }
    }
}