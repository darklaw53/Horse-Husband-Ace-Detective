using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Character", menuName = "Dialogue System/Dialogue Character")]
public class CharacterSO : ScriptableObject
{
    public string charName;
    public Sprite angry, sad, happy;
}