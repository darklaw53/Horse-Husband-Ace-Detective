using UnityEngine;

public class DontTouchWallsController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    public GameObject victoryScreen;
    public GameObject failScreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Goal")
        {
            victoryScreen.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            failScreen.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("UIFollowMouse: No Canvas found in parent hierarchy. Honestly, what did you expect?");
        }
    }

    void Update()
    {
        if (canvas == null) return;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out mousePos
        );

        rectTransform.anchoredPosition = mousePos;
    }
}