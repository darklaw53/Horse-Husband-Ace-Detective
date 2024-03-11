using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[SelectionBase]
public class CaracterController : MonoBehaviour
{
    #region Editor Data
    [Header("Movement Attributes")]
    [SerializeField] float _moveSpeed = 5f;

    public float tileSize = 1f;
    private Vector2 targetPosition, prevPos;
    bool isMoving = false;

    [Header("Dependencies")]
    Rigidbody2D _rb;
    #endregion

    //debug, delete later
    public GameObject marker;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        prevPos = transform.position;
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

        // Check if any movement input is pressed
        if (horizontalInput != 0 || verticalInput != 0)
        {
            // Start moving if not already moving
            if (!isMoving)
            {
                prevPos = transform.position;

                isMoving = true;
                Vector2Int adjacentTilePosition = GetCurrentTilePosition() + 
                    new Vector2Int(Mathf.RoundToInt(horizontalInput), Mathf.RoundToInt(verticalInput));

                // Calculate the center of the adjacent tile
                Vector3 adjacentTileCenter = new Vector3(adjacentTilePosition.x * tileSize + tileSize / 2,
                                                          adjacentTilePosition.y * tileSize + tileSize / 2,
                                                          0);
                targetPosition = adjacentTileCenter;


                //debug, delete later
                marker.transform.position = targetPosition;
            }
        }

        // Check if the player has reached the target position (center of tile)
        if (isMoving && (Vector2)transform.position == targetPosition)
        {
            isMoving = false;
        }
    }
    #endregion


    public Vector2Int GetCurrentTilePosition()
    {
        // Convert world position to grid coordinates
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        targetPosition = prevPos;

        //debug, delete later
        marker.transform.position = targetPosition;
    }
}