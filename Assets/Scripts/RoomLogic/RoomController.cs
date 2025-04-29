using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static RoomController Instance;

    public Transform roomParent;
    public GameObject currentRoom;
    public GameObject player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SwapRoom(GameObject newRoomPrefab, string entryDoorID)
    {
        if (currentRoom != null)
            Destroy(currentRoom);

        currentRoom = Instantiate(newRoomPrefab, Vector3.zero, Quaternion.identity, roomParent);

        DoorTarget target = FindTargetDoor(entryDoorID, currentRoom);
        if (target != null)
        {
            player.transform.position = target.spawnPoint.position;
        }
        else
        {
            Debug.LogError($"Door ID '{entryDoorID}' not found in new room. Genius move.");
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