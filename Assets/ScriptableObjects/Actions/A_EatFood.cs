using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Action/EatFood")]
public class A_EatFood : UtilityAction
{
    public override bool CanRun(UtilityAgent agent) => agent.inventoryFood > 0;

    public override IEnumerator Execute(UtilityAgent agent)
    {
        // Buscamos el granero mas cercano
        var granary = FindObjectOfType<Granary>();
        if (!granary) yield break;

        // Vamos al granero
        Vector3 target = granary.dropPoint != null ? granary.dropPoint.position : granary.transform.position;
        yield return agent.mover.GoTo(target, 1.2f);

        // Comemos
        agent.Blackboard.resources.TryConsumeFood(1);
        yield return new WaitForSeconds(4);        
    }
}
