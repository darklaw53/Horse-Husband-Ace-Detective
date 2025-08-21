using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DialogueTreeSO;

public class DialogueManager : Singleton<DialogueManager>
{
    public DialogueTreeSO currentDialogueTree;
    public DialogueNode currentDialogueNode;
    public TextMeshProUGUI textBox;

    public int currentLine = 0;

    public GameObject optionPrefab;
    public Transform optionHolder;

    List<GameObject> instantiatedButtons = new List<GameObject>();

    public GameObject chatBox;

    public void StartDialogue(DialogueTreeSO dialogue)
    {
        ThirdPersonController.Instance.moveEnabled = false;
        currentDialogueTree = dialogue;
        LoadNode(currentDialogueTree.GetStartNode());
        chatBox.SetActive(true);
    }

    public void LoadNode(DialogueNode node)
    {
        currentDialogueNode = node;
        UpdateText();
    }

    public void FinishDiologue()
    {
        chatBox.SetActive(false);
        ThirdPersonController.Instance.moveEnabled = true;
    }

    public void OnFinishedTyping()
    {
        if (currentDialogueNode.IsTerminalNode)
        {
            FinishDiologue();
            return;
        }

        if (currentDialogueNode.options.Count > 1)
            ExposeOptions();
        else if (currentDialogueNode.options.Count == 1)
            LoadNode(currentDialogueTree.GetNodeById(currentDialogueNode.ActionOption.nextNodeId));
        else
            FinishDiologue();
    }

    void UpdateText()
    {
        if (currentDialogueNode == null || textBox == null) return;
        textBox.SetText(currentDialogueNode.text);
        textBox.ForceMeshUpdate(true);
    }

    void ExposeOptions()
    {
        currentLine = 0;
        textBox.text = "";

        foreach (DialogueOption option in currentDialogueNode.options)
        {
            GameObject buttonGO = Instantiate(optionPrefab, optionHolder);
            OptionButton button = buttonGO.GetComponent<OptionButton>();
            button.text.text = option.optionText;

            button.button.onClick.AddListener(() => ButtonClicked(option));
            instantiatedButtons.Add(buttonGO);
        }
    }

    void ButtonClicked(DialogueOption option)
    {
        foreach (GameObject buttonGO in instantiatedButtons)
        {
            Destroy(buttonGO);
        }
        instantiatedButtons.Clear();

        DialogueNode nextNode = currentDialogueTree.GetNodeById(option.nextNodeId);
        if (nextNode != null)
            LoadNode(nextNode);
        else
            FinishDiologue();
    }

    public List<string> SplitString(string input)
    {
        List<string> output = new List<string>();

        string[] substrings = input.Split(new string[] { "\\next" }, System.StringSplitOptions.None);

        foreach (string substring in substrings)
        {
            output.Add(substring);
        }

        return output;
    }
}