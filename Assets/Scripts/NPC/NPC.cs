using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactabe
{
    public DialogueTreeSO currentDialogue;

    public override void Activate()
    {
        DialogueManager.Instance.StartDialogue(currentDialogue);
    }
}