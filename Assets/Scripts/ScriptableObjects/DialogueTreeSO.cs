using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Tree")]
public class DialogueTreeSO : ScriptableObject
{
    [Serializable]
    public class DialogueNode
    {
        public string id;
        [TextArea(3, 10)] public string text;
        public List<DialogueOption> options = new List<DialogueOption>();
        public Vector2 position;
        public bool isStartNode = false;

        public bool isActionNode;
        public List<string> actionIds;

        public DialogueOption ActionOption
        {
            get
            {
                var hiddenOption = options.Find(opt => opt.isHiddenForAction);
                if (hiddenOption == null)
                {
                    hiddenOption = new DialogueOption
                    {
                        optionText = "",
                        nextNodeId = "",
                        isHiddenForAction = true
                    };
                    options.Add(hiddenOption);
                }
                return hiddenOption;
            }
        }
    }

    [Serializable]
    public class DialogueOption
    {
        public string optionText;
        public string nextNodeId;

        public bool isHiddenForAction = false;
    }

    public List<DialogueNode> nodes = new List<DialogueNode>();

    public DialogueNode GetNodeById(string id)
    {
        return nodes.Find(n => n.id == id);
    }

    public DialogueNode GetStartNode()
    {
        return nodes.FirstOrDefault(n => n.isStartNode);
    }
}