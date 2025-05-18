using UnityEngine;

public class RoomController : Singleton<RoomController>
{
    public Transform roomParent;
    public GameObject currentRoom;
    public GameObject player;

    public LayerMask floorMask;

    public void SwapRoom(GameObject newRoomPrefab, string entryDoorID)
    {
        if (currentRoom != null)
            Destroy(currentRoom);

        currentRoom = Instantiate(newRoomPrefab, Vector3.zero, Quaternion.identity, roomParent);

        DoorTarget target = FindTargetDoor(entryDoorID, currentRoom);
        if (target != null)
        {
            Vector3 spawnPosition = target.spawnPoint.position;

            if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, 10f, floorMask))
            {
                spawnPosition.y = hit.point.y;
            }

            player.transform.position = spawnPosition;
        }
        else
        {
            Debug.LogError($"Door ID '{entryDoorID}' not found in new room. Brilliant. Just brilliant.");
        }
    }


    private DoorTarget FindTargetDoor(string id, GameObject room)
    {
        DoorTarget[] targets = room.GetComponentsInChildren<DoorTarget>();
        foreach (var target in targets)
        {
            if (target.doorID == id)
                return target;
        }
        return null;
    }
}