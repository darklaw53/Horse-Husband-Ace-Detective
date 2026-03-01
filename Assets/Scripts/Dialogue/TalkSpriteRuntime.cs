using UnityEngine;
using UnityEngine.UI;

public class TalkSpriteRuntime : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rect;
    [HideInInspector]
    public Image image;

    [HideInInspector]
    public RectTransform targetAnchor;

    Vector2 offscreenPos;
    float lerpSpeed = 8f;

    public void Initialize(Sprite sprite, Vector2 spawnPos)
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        image.sprite = sprite;
        rect.anchoredPosition = spawnPos;
        offscreenPos = spawnPos;
    }

    public void SetExpression(Sprite sprite)
    {
        if (sprite != null)
            image.sprite = sprite;
    }

    public void SetAnchor(RectTransform anchor)
    {
        targetAnchor = anchor;
    }

    public void ClearAnchor()
    {
        targetAnchor = null;
    }

    void Update()
    {
        Vector2 target;

        RectTransform parentRect = rect.parent as RectTransform;

        if (targetAnchor != null)
        {
            Vector2 localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                RectTransformUtility.WorldToScreenPoint(null, targetAnchor.position),
                null,
                out localPoint
            );

            target = localPoint;
        }
        else
        {
            target = ScreenToLocal(offscreenPos, parentRect);
        }

        rect.anchoredPosition = Vector2.Lerp(
            rect.anchoredPosition,
            target,
            Time.deltaTime * lerpSpeed
        );
    }

    void OnDisable()
    {
        RectTransform parentRect = rect.parent as RectTransform;

        rect.anchoredPosition = ScreenToLocal(offscreenPos, parentRect);
    }

    Vector2 ScreenToLocal(Vector2 screenPos, RectTransform parentRect)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPos,
            null,
            out localPoint
        );

        return localPoint;
    }
}