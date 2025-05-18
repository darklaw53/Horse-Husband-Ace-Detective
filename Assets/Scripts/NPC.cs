using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactabe
{
    public List<DialogueNodeSO> dialogues;

    public override void Activate()
    {
        DialogueManager.Instance.StartDialogue(dialogues[0]);
        if (dialogues.Count > 1) dialogues.RemoveAt(0);
    }
}