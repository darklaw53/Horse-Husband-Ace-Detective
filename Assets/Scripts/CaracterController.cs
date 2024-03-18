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
public class CaracterController : MonoBehaviour
{
    public float _moveSpeed = 5f;
    public float tileSize = 1f;

    private Vector2 targetPosition;
    public Direction lookDir;

    public bool northCol, southCol, eastCol, westCol;

    Rigidbody2D _rb;
    bool isMoving = false;

    //debug, delete later
    public GameObject marker;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    #region Tick
    private void Update()
    {
        GatherInput();
    }

    private void FixedUpdate()
    {
        MovementUpdate();
    }
    #endregion

    #region Input Logic
    void GatherInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            if (!isMoving)
            {
                int horizontalMove = Mathf.RoundToInt(horizontalInput);
                int verticalMove = Mathf.RoundToInt(verticalInput);

                Vector2Int adjacentTilePosition = GetCurrentTilePosition();

                if ((horizontalMove == 1 && !eastCol) || (horizontalMove == -1 && !westCol))
                {
                    isMoving = true;

                    adjacentTilePosition = adjacentTilePosition + new Vector2Int(horizontalMove, 0);


                    if (horizontalMove == 1) lookDir = Direction.East;
                    else lookDir = Direction.West;
                }

                if ((verticalMove == 1 && !northCol) || (verticalMove == -1 && !southCol))
                {
                    isMoving = true;

                    adjacentTilePosition = adjacentTilePosition + new Vector2Int(0, verticalMove);

                    if (verticalMove == 1) lookDir = Direction.North;
                    else lookDir = Direction.South;
                }

                Vector3 adjacentTileCenter = new Vector3(adjacentTilePosition.x * tileSize + tileSize / 2,
                                                              adjacentTilePosition.y * tileSize + tileSize / 2, 0);

                targetPosition = adjacentTileCenter;

                if (marker != null) marker.transform.position = targetPosition;
            }
        }

        if (isMoving && (Vector2)transform.position == targetPosition)
        {
            isMoving = false;
        }
    }

    #endregion

    public Vector2Int GetCurrentTilePosition()
    {
        Vector2Int tilePosition = new Vector2Int(
            Mathf.FloorToInt(transform.position.x / tileSize),
            Mathf.FloorToInt(transform.position.y / tileSize)
        );

        return tilePosition;
    }

    #region Movement Logic
    void MovementUpdate()
    {
        if (isMoving)
        {
            Vector2 newPosition = Vector2.MoveTowards(_rb.position, targetPosition, _moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(newPosition);
        }
    }
    #endregion
}