using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Interactabe : MonoBehaviour
{
    [HideInInspector]public Vector3Int cellPosition;

    public virtual void Start()
    {
        CenterGameObject();
    }

    private void CenterGameObject()
    {
        if (TileMapManager.Instance.background == null)
        {
            Debug.LogError("Tilemap is not assigned on " + gameObject.name);
            return;
        }

        cellPosition = TileMapManager.Instance.background.WorldToCell(transform.position);

        Vector3 cellCenterWorldPosition = TileMapManager.Instance.background.GetCellCenterWorld(cellPosition);

        transform.position = cellCenterWorldPosition;
    }

    public void ReCenter()
    {
        CenterGameObject();
    }

    public virtual void Activate()
    {

    }
}