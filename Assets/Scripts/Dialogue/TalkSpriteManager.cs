using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static DialogueTreeSO;

public class TalkSpriteManager : MonoBehaviour
{
    [Header("Layout Parents")]
    public RectTransform leftLayout;
    public RectTransform rightLayout;

    [Header("Visible Sprite Holder")]
    public RectTransform spriteHolder;

    [Header("Prefabs")]
    public GameObject anchorPrefab;     // Transparent Image
    public GameObject talkSpritePrefab; // Visible sprite

    public Dictionary<string, TalkSpriteRuntime> activeSprites =
        new Dictionary<string, TalkSpriteRuntime>();

    public Dictionary<string, RectTransform> activeAnchors =
        new Dictionary<string, RectTransform>();

    public Transform spawnPointLeft, spawnPointRight;

    public void ApplyDialogueNode(DialogueNode node)
    {
        foreach (CharacterInScene charState in node.characters)
        {
            RectTransform layoutParent =
                charState.side == HorizDirection.Left
                ? leftLayout
                : rightLayout;

            RectTransform anchor;

            if (!activeAnchors.ContainsKey(charState.character.characterName))
            {
                // Create anchor
                GameObject a = Instantiate(anchorPrefab, layoutParent);
                anchor = a.GetComponent<RectTransform>();
                activeAnchors.Add(charState.character.characterName, anchor);
            }
            else
            {
                anchor = activeAnchors[charState.character.characterName];

                // Move anchor if side changed
                if (anchor.parent != layoutParent)
                    anchor.SetParent(layoutParent);
            }

            TalkSpriteRuntime runtime;

            if (!activeSprites.ContainsKey(charState.character.characterName))
            {
                GameObject obj =
                    Instantiate(talkSpritePrefab, spriteHolder);

                runtime = obj.GetComponent<TalkSpriteRuntime>();

                runtime.Initialize(
                    GetSprite(charState.character),
                    charState.side == HorizDirection.Left
                    ? spawnPointLeft.position
                    : spawnPointRight.position
                );

                runtime.transform.position = (charState.side == HorizDirection.Left
                    ? spawnPointLeft.position
                    : spawnPointRight.position);

                activeSprites.Add(charState.character.characterName, runtime);
            }
            else
            {
                runtime = activeSprites[charState.character.characterName];
                runtime.SetExpression(GetSprite(charState.character));
            }

            if (charState.characterSceneState == CharacterSceneState.Leave)
                if (activeSprites.ContainsKey(charState.character.name))
                    CharacterLeaves(charState.character.name);

            runtime.SetAnchor(anchor);
        }
    }

    public void CharacterLeaves(string characterName)
    {
        if (activeAnchors.TryGetValue(characterName, out var anchor))
        {
            if (anchor) Destroy(anchor.gameObject);
            activeAnchors.Remove(characterName);
        }
    }

    public Sprite GetSprite(CharacterSO character)
    {
        Expression expression = character.currentExpression;

        foreach (ExpressionSprite entry in character.talkSprites)
        {
            if (entry.expression == expression)
            {
                return entry.sprite;
            }
        }

        // Fallback if not found
        Debug.LogWarning($"Expression {expression} not found for {character.name}");
        return null;
    }
}