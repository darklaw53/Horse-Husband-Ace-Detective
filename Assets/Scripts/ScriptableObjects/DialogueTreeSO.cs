using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HorizDirection
{
    Left,
    Right
}

public enum CharacterSceneState
{
    Stay,
    Leave
}

[Serializable]
public struct CharacterInScene
{
    public CharacterSO character;
    public HorizDirection side;
    public CharacterSceneState characterSceneState;
    public Expression expression;
}

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
        public List<string> actionCommandIds = new();

        public List<CharacterInScene> characters = new();
        public CharacterSO speaker;

        public float cachedHeight = 200f;

        public DialogueOption ActionOption
        {
            get
            {
                DialogueOption hiddenOption = options.Find(opt => opt.isHiddenForAction);
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

        public bool IsTerminalNode
        {
            get
            {
                if (options == null || options.Count == 0)
                    return true;

                return options.All(opt => string.IsNullOrEmpty(opt.nextNodeId));
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