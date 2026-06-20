using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Granja donde los agentes (trabajadores) pueden venir a producir comida.
// Implementa dos interfaces:
// - IWorkplace (Para gestionar trabajadores)
// IResourceSource (Para producir recursos)
public class Farm : MonoBehaviour, IWorkplace, IResourceSource
{
    // CONFIG GRANJA ------------------------------------------------
    [SerializeField] private WorkSpot[] spots; // Puntos de trabajo donde se colocan los aldeanos
    [SerializeField] private float workSeconds = 4f;
    [SerializeField] private int yieldPerCycle = 4; // Cantidad de comida producidapor ciclo de trabajo

    // PROPIEDADES REQUERIDAS POR INTERFACES ------------------------
    public string ResourceId => "Food";
    public float WorkSeconds => workSeconds;    

    // GESTION PUESTOS TRABAJO --------------------------------------
    /// <summary>
    /// Intenta reservar un puesto libre para un trabajador
    /// </summary>
    /// <param name="worker">Trabajador</param>
    /// <param name="spot">Devuelve el Transform del spot en caso de estar libre, si no, null</param>
    /// <returns></returns>
    public bool TryReserveSpot(GameObject worker, out Transform spot)
    {
        foreach(var s in spots)
        {
            // Si esta libre
            if(s.reservedBy == null)
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
            if (s.reservedBy == null) return true;
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
