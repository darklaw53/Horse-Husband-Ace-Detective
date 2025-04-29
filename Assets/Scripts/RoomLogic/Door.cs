using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject targetRoomPrefab;
    public string targetDoorID;

    public void OnPlayerInteract()
    {
        RoomController.Instance.SwapRoom(targetRoomPrefab, targetDoorID);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnPlayerInteract();
        }
    }
}