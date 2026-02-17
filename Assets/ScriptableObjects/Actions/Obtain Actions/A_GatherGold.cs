using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Action/GatherGold")]
public class A_GatherGold : UtilityAction
{
    public float searchRadius = 80f;

    public override bool CanRun(UtilityAgent agent)
    {
        if (agent.inventoryGold >= agent.maxGold) return false;
        return Globals.FindNearestMine(agent.transform.position, searchRadius, MineType.Gold) != null;
    }

    public override IEnumerator Execute(UtilityAgent agent)
    {
        var mine = Globals.FindNearestMine(agent.transform.position, searchRadius, MineType.Gold);
        if (!mine) yield break;

        if (!mine.TryReserveSpot(agent.gameObject, out var spot))
            yield break;

        yield return agent.mover.GoTo(spot.position, 0.85f);
        yield return new WaitForSeconds(mine.WorkSeconds);

        int gained = mine.Harvest(agent.gameObject);
        int free = Mathf.Max(0, agent.maxGold - agent.inventoryGold);
        int take = Mathf.Min(free, gained);
        agent.inventoryGold += take;

        mine.ReleaseSpot(agent.gameObject);
    }    
}
