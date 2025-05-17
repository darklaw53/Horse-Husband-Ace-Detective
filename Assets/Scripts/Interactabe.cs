using UnityEngine;

public class Interactabe : MonoBehaviour
{
    public GameObject notificationSprite;

    public void Select()
    {
        notificationSprite.SetActive(true);
    }

    public void UnSelect()
    {
        notificationSprite.SetActive(false);
    }

    public virtual void Activate(){}
}