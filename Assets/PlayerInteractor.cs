using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public ThirdPersonController caracterController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            caracterController.targetInteractable = null;
            caracterController.targetInteractable = other.GetComponent<Interactabe>();
            caracterController.targetInteractable.Select();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            if (other.gameObject == caracterController.targetInteractable)
            {
                caracterController.targetInteractable.UnSelect();
                caracterController.targetInteractable = null;
            }
        }
    }
}