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
        if (agent.inventoryFood >= agent.carryCapacity) return false; // Inventario lleno
        return FindNearest<Farm>(agent.transform.position, searchRadius) != null;
    }

    public override IEnumerator Execute(UtilityAgent agent)
    {
        var farm = FindNearest<Farm>(agent.transform.position, searchRadius);
        if (!farm) yield break;
        if (!farm.TryReserveSpot(agent.gameObject, out var spot)) yield break;

        // Ir al punto de trabajo
        yield return agent.mover.GoTo(spot.position, 0.85f); // [POS, STOPPINGDIST]

        // “Trabajar” (simulado por espera)
        yield return new WaitForSeconds(farm.WorkSeconds);

        // Cosecha
        int gained = farm.Harvest(agent.gameObject);

        // Añadir al inventario respetando capacidad
        int free = Mathf.Max(0, agent.carryCapacity - agent.inventoryFood);
        int toTake = Mathf.Min(free, gained);
        agent.inventoryFood += toTake;

        farm.ReleaseSpot(agent.gameObject);
    }

    // ---- Helpers ----
    static T FindNearest<T>(Vector3 from, float radius) where T : Component
    {
        var all = Object.FindObjectsOfType<T>();
        float r2 = radius * radius;
        T best = null; float bestD2 = float.MaxValue;
        foreach (var t in all)
        {
            float d2 = (t.transform.position - from).sqrMagnitude;
            if (d2 <= r2 && d2 < bestD2) { bestD2 = d2; best = t; }
        }
        return best;
    }
}
