using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Action/DeliverWood")]
public class A_DeliverWood : UtilityAction
{
    public override bool CanRun(UtilityAgent agent) => agent.inventoryWood > 0;

    public override IEnumerator Execute(UtilityAgent agent)
    {
        var sawmill = FindObjectOfType<SawMill>();
        if (!sawmill) yield break;

        Vector3 target = sawmill.dropPoint != null? sawmill.dropPoint.position : sawmill.transform.position; // Por si no detecta los dropPoints
        yield return agent.mover.GoTo(target, 1.2f);

        int amount = agent.inventoryWood;
        if(amount > 0)
        {
            agent.Blackboard.resources.TryAddWood(amount);
            agent.inventoryWood = 0;
        }
    }
}
