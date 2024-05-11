using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "Dialogue System/Dialogue Node")]
public class DialogueNodeSO : ScriptableObject
{
    public List<LineSO> lines;
    public List<DialogueOptionSO> options = new List<DialogueOptionSO>();
}