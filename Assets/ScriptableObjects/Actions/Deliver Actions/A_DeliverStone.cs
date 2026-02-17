using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Action/DeliverStone")]

public class A_DeliverStone : UtilityAction
{
    public override bool CanRun(UtilityAgent agent) => agent.inventoryStone > 0;

    public override IEnumerator Execute(UtilityAgent agent)
    {
        var os = FindObjectOfType<OresStorage>();
        if (!os) yield break;

        Vector3 target = os.dropPoint ? os.dropPoint.position : os.transform.position;
        yield return agent.mover.GoTo(target, 1.2f);

        // Depositar el inventario
        int amount = agent.inventoryStone;
        agent.Blackboard.resources.TryAddStone(amount);
        agent.inventoryStone = 0;
        os.UpdateGameObjectVisuals(); // Actualizar visualmente el almacen de ores
    }
}
