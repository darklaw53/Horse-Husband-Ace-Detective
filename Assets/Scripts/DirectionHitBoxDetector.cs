using UnityEngine;
using UnityEngine.Windows;

public class ColliderOverlapChecker : MonoBehaviour
{
    public Collider2D colNorth, colSouth, colWest, colEast;

    public CaracterController caracterController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == colNorth)
        {
            caracterController.northCol = true;
        }
        else if (other == colSouth)
        {
            caracterController.southCol = true;
        }
        else if (other == colWest)
        {
            caracterController.westCol = true;
        }
        else if (other == colEast)
        {
            caracterController.eastCol = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == colNorth)
        {
            caracterController.northCol = false;
        }
        else if (other == colSouth)
        {
            caracterController.southCol = false;
        }
        else if (other == colWest)
        {
            caracterController.westCol = false;
        }
        else if (other == colEast)
        {
            caracterController.eastCol = false;
        }
    }
}