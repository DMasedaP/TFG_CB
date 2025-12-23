using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IWorkplace, IResourceSource
{
    [SerializeField] private WorkSpot[] spots;
    [SerializeField] private float workSeconds = 5;
    [SerializeField] private int yieldPerCycle = 1; // Cantidad producida

    // PROPIEDADES REQUERIDAS POR INTERFACES ------------------------
    public string ResourceId => "Wood";
    public float WorkSeconds => workSeconds;

    // GESTION PUESTOS TRABAJO --------------------------------------
    public bool TryReserveSpot(GameObject worker, out Transform spot)
    {
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
    public int Harvest(GameObject worker) => yieldPerCycle;
}
