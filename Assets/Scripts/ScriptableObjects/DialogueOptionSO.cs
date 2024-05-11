using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Option", menuName = "Dialogue System/Dialogue Option")]
public class DialogueOptionSO : ScriptableObject
{
    public string text;
    public DialogueNodeSO nextNode;
}