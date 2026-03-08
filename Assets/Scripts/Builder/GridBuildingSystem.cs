using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;
    [SerializeField] private Vector3 originPos = Vector3.zero;

    private bool[,] occupied; // Para controlar las celdas ocupadas

    private void Awake()
    {
        occupied = new bool[width, height];
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 localPos = worldPosition - originPos;
        x = Mathf.FloorToInt(localPos.x / cellSize);
        y = Mathf.FloorToInt(localPos.z / cellSize);
    }

    public Vector3 GetCellCenterWorld(int x, int y)
    {
        return originPos + new Vector3((x + 0.5f) * cellSize, 0f, (y + 0.5f) * cellSize);
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public bool CanPlaceBuilding(int startX, int startY, int buildingWidth, int buildingHeight)
    {
        for (int x = startX; x < startX + buildingWidth; x++)
        {
            for (int y = startY; y < startY + buildingHeight; y++)
            {
                if (!IsInsideGrid(x, y))
                    return false;

                if (occupied[x, y])
                    return false;
            }
        }

        return true;
    }

    public void SetOccupied(int startX, int startY, int buildingWidth, int buildingHeight, bool value)
    {
        for (int x = startX; x < startX + buildingWidth; x++)
        {
            for (int y = startY; y < startY + buildingHeight; y++)
            {
                if (IsInsideGrid(x, y))
                    occupied[x, y] = value;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        for (int x = 0; x <= width; x++)
        {
            Vector3 start = originPos + new Vector3(x * cellSize, 0f, 0f);
            Vector3 end = originPos + new Vector3(x * cellSize, 0f, height * cellSize);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= height; y++)
        {
            Vector3 start = originPos + new Vector3(0f, 0f, y * cellSize);
            Vector3 end = originPos + new Vector3(width * cellSize, 0f, y * cellSize);
            Gizmos.DrawLine(start, end);
        }
    }
}
