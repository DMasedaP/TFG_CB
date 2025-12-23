using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="TFG/US/Action/HarvestFarm")]
public class A_HarvestFarm : UtilityAction
{
    [Header("Busqueda")]
    public float searchRadius = 60f;

    public override bool CanRun(UtilityAgent agent)
    {
        if (agent.inventoryFood >= agent.maxFood) return false; // Inventario lleno
        return Globals.FindNearestWithFreeSpot<Farm>(agent.transform.position, searchRadius) != null;
    }

    public override IEnumerator Execute(UtilityAgent agent)
    {
        var farm = Globals.FindNearestWithFreeSpot<Farm>(agent.transform.position, searchRadius);
        if (!farm) yield break;
        if (!farm.TryReserveSpot(agent.gameObject, out var spot)) yield break;

        // Ir al punto de trabajo
        yield return agent.mover.GoTo(spot.position, 0.85f); // [POS, STOPPINGDIST]

        // “Trabajar” (simulado por espera)
        yield return new WaitForSeconds(farm.WorkSeconds);

        // Cosecha
        int gained = farm.Harvest(agent.gameObject);

        // Añadir al inventario respetando capacidad
        int free = Mathf.Max(0, agent.maxFood - agent.inventoryFood);
        int toTake = Mathf.Min(free, gained);
        agent.inventoryFood += toTake;

        farm.ReleaseSpot(agent.gameObject);
    }
}
