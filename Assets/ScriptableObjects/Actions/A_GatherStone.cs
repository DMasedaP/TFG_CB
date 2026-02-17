using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Action/GatherStone")]

public class A_GatherStone : UtilityAction
{
    public float searchRadius = 80f;

    public override bool CanRun(UtilityAgent agent)
    {
        if (agent.inventoryStone >= agent.maxStone) return false;
        return Globals.FindNearestMine(agent.transform.position, searchRadius, MineType.Stone) != null;
    }

    public override IEnumerator Execute(UtilityAgent agent)
    {
        var mine = Globals.FindNearestMine(agent.transform.position, searchRadius, MineType.Stone);
        if (!mine) yield break;

        if (!mine.TryReserveSpot(agent.gameObject, out var spot))
            yield break;

        yield return agent.mover.GoTo(spot.position, 0.85f);
        yield return new WaitForSeconds(mine.WorkSeconds);

        int gained = mine.Harvest(agent.gameObject);
        int free = Mathf.Max(0, agent.maxStone - agent.inventoryStone);
        int take = Mathf.Min(free, gained);
        agent.inventoryStone += take;

        mine.ReleaseSpot(agent.gameObject);
    }
}
