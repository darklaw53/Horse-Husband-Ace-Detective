using UnityEngine;

public class SendObjectToFloor : MonoBehaviour
{
    private void Start()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, RoomController.Instance.floorMask))
        {
            transform.position = hit.point;
        }
    }
}