using UnityEngine;
using UnityEngine.Windows;

public class ColliderOverlapChecker : MonoBehaviour
{
    public Direction dir;

    public CaracterController caracterController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            switch (dir)
            {
                case Direction.North:
                    caracterController.northCol = true;
                    break;
                case Direction.South:
                    caracterController.southCol = true;
                    break;
                case Direction.West:
                    caracterController.westCol = true;
                    break;
                case Direction.East:
                    caracterController.eastCol = true;
                    break;
            }
        }

        if (other.CompareTag("Interactable"))
        {
            switch (dir)
            {
                case Direction.North:
                    caracterController.northObj = other.gameObject;
                    break;
                case Direction.South:
                    caracterController.southObj = other.gameObject;
                    break;
                case Direction.West:
                    caracterController.westObj = other.gameObject;
                    break;
                case Direction.East:
                    caracterController.eastObj = other.gameObject;
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            switch (dir)
            {
                case Direction.North:
                    caracterController.northCol = false;
                    break;
                case Direction.South:
                    caracterController.southCol = false;
                    break;
                case Direction.West:
                    caracterController.westCol = false;
                    break;
                case Direction.East:
                    caracterController.eastCol = false;
                    break;
            }
        }

        if (other.CompareTag("Interactable"))
        {
            switch (dir)
            {
                case Direction.North:
                    if (other.gameObject == caracterController.northObj)
                    {
                        caracterController.northObj = null;
                    }
                    break;
                case Direction.South:
                    if (other.gameObject == caracterController.southObj)
                    {
                        caracterController.southObj = null;
                    }
                    break;
                case Direction.West:
                    if (other.gameObject == caracterController.westObj)
                    {
                        caracterController.westObj = null;
                    }
                    break;
                case Direction.East:
                    if (other.gameObject == caracterController.eastObj)
                    {
                        caracterController.eastObj = null;
                    }
                    break;
            }
        }
    }
}