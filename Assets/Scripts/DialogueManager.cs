using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    public DialogueNodeSO currentDialogueNode;
    public TextMeshProUGUI textBox;

    public int currentLine = 0;

    public GameObject optionPrefab;
    public Transform optionHolder;

    List<GameObject> instantiatedButtons = new List<GameObject>();

    public GameObject chatBox;

    public void StartDialogue(DialogueNodeSO dialogue)
    {
        ThirdPersonController.Instance.moveEnabled = false;
        currentLine = 0;
        currentDialogueNode = dialogue;
        UpdateText();
        chatBox.SetActive(true);
    }

    public void FinishDiologue()
    {
        chatBox.SetActive(false);
        ThirdPersonController.Instance.moveEnabled = true;
    }

    public void NextLine()
    {
        currentLine++;
        if (currentLine < currentDialogueNode.lines.Count) UpdateText();
        else if (currentDialogueNode.options.Count > 0) ExposeOptions();
        else FinishDiologue();
    }

    void UpdateText()
    {
        textBox.SetText(currentDialogueNode.lines[currentLine].text);
        textBox.ForceMeshUpdate(true);
    }

    void ExposeOptions()
    {
        currentLine = 0;
        textBox.text = "";

        foreach (DialogueOptionSO option in currentDialogueNode.options) 
        {
            GameObject buttonGO = Instantiate(optionPrefab, optionHolder);
            Button button = buttonGO.GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = option.text;

            button.onClick.AddListener(() => ButtonClicked(option));
            instantiatedButtons.Add(buttonGO);
        }
    }

    void ButtonClicked(DialogueOptionSO option)
    {
        foreach (GameObject buttonGO in instantiatedButtons)
        {
            Destroy(buttonGO);
        }
        instantiatedButtons.Clear();

        StartDialogue(option.nextNode);
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