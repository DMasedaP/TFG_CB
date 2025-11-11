using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="TFG/US/Action/DeliverFood")]
public class A_DeliverFood : UtilityAction
{
    public override bool CanRun(UtilityAgent agent) => agent.inventoryFood > 0;

    public override IEnumerator Execute(UtilityAgent agent)
    {
        var granary = Object.FindObjectOfType<Granary>();
        if (!granary) yield break;

        // Ir al punto de entrega
        Vector3 target = granary.dropPoint != null ? granary.dropPoint.position : granary.transform.position;
        yield return agent.mover.GoTo(target, 1.2f);

        // Depositar todo el inventario en GameManager
        int amount = agent.inventoryFood;
        if (amount > 0)
        {
            agent.Blackboard.resources.TryAddFood(amount); // GameManager
            Debug.Log($"[DeliverFood] Depositing {amount} to GameManager");
            agent.inventoryFood = 0;
        }
    }
}
