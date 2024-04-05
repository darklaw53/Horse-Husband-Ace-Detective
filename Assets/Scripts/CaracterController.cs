using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Direction
{
    North,
    South,
    East,
    West
}

[SelectionBase]
public class CaracterController : Singleton<CaracterController>
{
    public float _moveSpeed = 5f;
    public float tileSize = 1f;

    private Vector2 targetPosition;
    public Direction lookDir;

    public bool northCol, southCol, eastCol, westCol;
    [HideInInspector]public GameObject northObj, southObj, eastObj, westObj;

    Rigidbody2D _rb;
    bool isMoving = false;
    float horizontalInput, verticalInput;

    //debug, delete later
    public GameObject marker;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GatherInput();
    }

    private void FixedUpdate()
    {
        MovementUpdate();
    }

    void GatherInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            CalculateTargetPosition();
        }

        if (isMoving && (Vector2)transform.position == targetPosition)
        {
            isMoving = false;
        }

        if (Input.GetKeyDown(KeyCode.E)) Interact();
    }

    public Vector2Int GetCurrentTilePosition()
    {
        Vector2Int tilePosition = new Vector2Int(
            Mathf.FloorToInt(transform.position.x / tileSize),
            Mathf.FloorToInt(transform.position.y / tileSize)
        );

        return tilePosition;
    }

    void Interact()
    {
        if (!isMoving) 
        {
            switch (lookDir)
            {
                case Direction.North:
                    if (northObj != null) northObj.GetComponent<Interactabe>().Activate();
                    break;
                case Direction.South:
                    if (southObj != null) southObj.GetComponent<Interactabe>().Activate();
                    break;
                case Direction.West:
                    if (westObj != null) westObj.GetComponent<Interactabe>().Activate();
                    break;
                case Direction.East:
                    if (eastObj != null) eastObj.GetComponent<Interactabe>().Activate();
                    break;
            }
        }
    }

    void CalculateTargetPosition()
    {
        if (!isMoving)
        {
            int horizontalMove = Mathf.RoundToInt(horizontalInput);
            int verticalMove = Mathf.RoundToInt(verticalInput);

            Vector2Int adjacentTilePosition = GetCurrentTilePosition();

            if (horizontalMove != 0 && ((horizontalMove == 1 && !eastCol) || (horizontalMove == -1 && !westCol)))
            {
                isMoving = true;
                adjacentTilePosition += new Vector2Int(horizontalMove, 0);
                lookDir = horizontalMove == 1 ? Direction.East : Direction.West;
            }
            else if (verticalMove != 0 && ((verticalMove == 1 && !northCol) || (verticalMove == -1 && !southCol)))
            {
                isMoving = true;
                adjacentTilePosition += new Vector2Int(0, verticalMove);
                lookDir = verticalMove == 1 ? Direction.North : Direction.South;
            }

            if (horizontalMove != 0 || verticalMove != 0)
            {
                Vector3 adjacentTileCenter = new Vector3(adjacentTilePosition.x * tileSize + tileSize / 2,
                                                          adjacentTilePosition.y * tileSize + tileSize / 2, 0);
                targetPosition = adjacentTileCenter;

                if (marker != null)
                    marker.transform.position = targetPosition;
            }
        }
    }

    void MovementUpdate()
    {
        if (isMoving)
        {
            Vector2 newPosition = Vector2.MoveTowards(_rb.position, targetPosition, _moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(newPosition);
        }
    }
}