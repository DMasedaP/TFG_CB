using UnityEngine;

public enum MineType { Stone, Gold }

public class MineSite : MonoBehaviour, IWorkplace, IResourceSource
{
    [SerializeField] private MineType type;
    [SerializeField] private WorkSpot[] spots;
    [SerializeField] private float workSeconds => type == MineType.Stone ? 4 : 6; // Dependiendo si es piedra u oro, tarda mas o menos
    [SerializeField] private int yieldPerCycle = 1;

    [SerializeField] private GameObject mineChild; // Arrastrar
    [SerializeField] public bool constructed;
    // PROPIEDADES REQUERIDAS POR INTERFACES ------------------------
    public string ResourceId => type == MineType.Gold ? "Gold" : "Stone";
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


    // FUNCIONES PROPIAS --------------------------------------------
    public bool CanConstruct()
    {
        return !constructed;
    }
    public void Construct()
    {
        if (!CanConstruct()) return;
        constructed = true;
        mineChild.SetActive(true);
        // Aqui se controlara el coste etc...
        // ...
        // ...
        // ...
    }
}
