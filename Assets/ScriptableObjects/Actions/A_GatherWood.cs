using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Action/GatherWood")]
public class A_GatherWood : UtilityAction
{
    [Header("Busqueda")]
    public float searchRadius = 60f;
    public override bool CanRun(UtilityAgent agent)
    {
        if (agent.inventoryWood >= agent.maxWood) return false; // Inventario lleno
        return Globals.FindNearest<Tree>(agent.transform.position, searchRadius) != null; // 60 = searchRadius
    }
    public override IEnumerator Execute(UtilityAgent agent)
    {
        var tree = Globals.FindNearest<Tree>(agent.transform.position, searchRadius);
        if (!tree) yield break;
        if (!tree.TryReserveSpot(agent.gameObject, out var spot)) yield break;

        // Voy al arbol
        yield return agent.mover.GoTo(spot.position, 0.85f);
        // Simulamos el trabajo
        yield return new WaitForSeconds(tree.WorkSeconds);

        // Obtenemos madera
        int gained = tree.Harvest(agent.gameObject);
        // Añadimos al inventario
        int free = Mathf.Max(0, agent.maxWood - agent.inventoryWood);
        int toTake = Mathf.Min(free, gained);
        agent.inventoryWood += toTake;

        tree.ReleaseSpot(agent.gameObject);
    }
}
