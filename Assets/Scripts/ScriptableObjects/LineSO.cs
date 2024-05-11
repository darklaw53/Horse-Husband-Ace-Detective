using UnityEngine;

public enum Emotion
{
    Happy,
    Sad,
    Angry
}

[CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Dialogue System/Dialogue Line")]
public class LineSO : ScriptableObject
{
    public string text;
    public CharacterSO character;
    public Emotion emotion;
    public bool quick;
    public bool right;
}