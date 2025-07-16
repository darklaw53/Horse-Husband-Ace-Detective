using UnityEngine;

public class CableButton : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    public string enterTriggerName = "OnEnter";
    public string exitTriggerName = "OnExit";

    [Header("Light")]
    public Light targetLight;
    public Color colorOnEnter = Color.red;
    public Color colorOnExit = Color.white;

    [Header("Material Change")]
    public Renderer targetObject;
    public Material materialOnEnter;
    public Material materialOnExit;

    private int objectsInside = 0;

    private void OnTriggerEnter(Collider other)
    {
        objectsInside++;
        if (objectsInside == 1)
        {
            if (animator != null)
                animator.SetTrigger(enterTriggerName);

            if (targetLight != null)
                targetLight.color = colorOnEnter;

            if (targetObject != null && materialOnEnter != null)
                targetObject.material = materialOnEnter;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        objectsInside = Mathf.Max(0, objectsInside - 1);

        if (objectsInside == 0)
        {
            if (animator != null)
                animator.SetTrigger(exitTriggerName);

            if (targetLight != null)
                targetLight.color = colorOnExit;

            if (targetObject != null && materialOnExit != null)
                targetObject.material = materialOnExit;
        }
    }
}