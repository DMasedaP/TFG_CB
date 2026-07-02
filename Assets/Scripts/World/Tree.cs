using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IWorkplace, IResourceSource
{
    [Header("Work")]
    [SerializeField] private WorkSpot[] spots;
    [SerializeField] private float workSeconds = 5;
    [SerializeField] private int yieldPerCycle = 2; // Cantidad producida

    [Header("Health")]
    [SerializeField] private int maxHarvestCycles = 5;
    
    private int currentHarvestCycles = 0;
    private bool isDepleted = false;

    private GridBuildingSystem gridSystem;
    private int gridX;
    private int gridY;
    private int gridWidth = 1;
    private int gridHeight = 1;
    private bool isRegisteredInGrid = false;

    // PROPIEDADES REQUERIDAS POR INTERFACES ------------------------
    public string ResourceId => "Wood";
    public float WorkSeconds => workSeconds;

    // SPAWN ARBOL EN GRID ------------------------------------------
    public void RegisterInGrid(GridBuildingSystem grid, int x, int y, int width, int height)
    {
        gridSystem = grid;
        gridX = x;
        gridY = y;
        gridWidth = width;
        gridHeight = height;
        isRegisteredInGrid = true;
    }

    // GESTION PUESTOS TRABAJO --------------------------------------
    public bool TryReserveSpot(GameObject worker, out Transform spot)
    {
        if (isDepleted)
        {
            spot = null;
            return false;
        }
        foreach (var s in spots)
        {
            // Si esta libre
            if (s.reservedBy == null)
            {
                s.reservedBy = worker;
                spot = s.transform;
                return true;
            }
        }
        // Si no hay puestos libres devuelve false
        spot = null;
        return false;
    }

    public bool HasFreeSpot()
    {
        if (isDepleted)
            return false;
        foreach (var s in spots)
        {
            if (s.reservedBy == null) return true;
        }
        return false;        
    }

    /// <summary>
    /// Libera el puesto ocupado por un trabajador cuando termina su tarea o se va.
    /// </summary>
    public void ReleaseSpot(GameObject worker)
    {
        foreach (var s in spots)
        {
            if (s.reservedBy == worker)
                s.reservedBy = null;
        }
    }
    /// <returns>Cantidad de recursos generados en un ciclo de trabajo.</returns>
    public int Harvest(GameObject worker)
    {
        if (isDepleted)
            return 0;
        currentHarvestCycles++;

        if (currentHarvestCycles >= maxHarvestCycles)
        {
            CutDownTree();
        }

        return yieldPerCycle;
    }
    private void CutDownTree()
    {
        isDepleted = true;
        ReleaseAllSpots();

        Destroy(gameObject);
    }
    private void ReleaseAllSpots()
    {
        foreach (var s in spots)
        {
            s.reservedBy = null;
        }
    }
    private void OnDestroy()
    {
        if (!isRegisteredInGrid || gridSystem == null)
            return;

        gridSystem.SetOccupied(gridX, gridY, gridWidth, gridHeight, false);
    }
}
