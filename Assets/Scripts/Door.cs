using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : Interactabe
{
    public TileBase openTile;
    public TileBase closedTile;

    private bool isOpen = false;

    public override void Start()
    {
        base.Start();

        TileMapManager.Instance.wall.SetTile(cellPosition, closedTile);
    }

    public override void Activate()
    {
        isOpen = !isOpen;
        TileMapManager.Instance.wall.SetTile(cellPosition, isOpen ? null : closedTile);
        TileMapManager.Instance.intangirbleInteractableTilemap.SetTile(cellPosition, isOpen ? openTile : null);
    }
}