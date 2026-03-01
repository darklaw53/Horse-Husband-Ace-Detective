using System.Collections.Generic;
using UnityEngine;
public enum Expression
{
    Neutral,
    Happy,
    Angry,
    Sad,
    Surprised,
    Thinking,
    Bashful
}

[System.Serializable]
public struct ExpressionSprite
{
    public Expression expression;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "New Dialogue Character", menuName = "Dialogue System/Dialogue Character")]
public class CharacterSO : ScriptableObject
{
    public string characterName;
    public List<ExpressionSprite> talkSprites = new();
    public GameObject prefab;
    public Expression currentExpression;
}