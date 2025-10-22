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
            caracterController.targetInteractable.UnSelect();

            if (other.gameObject == caracterController.targetInteractable)
            {
                caracterController.targetInteractable = null;
            }
        }
    }
}