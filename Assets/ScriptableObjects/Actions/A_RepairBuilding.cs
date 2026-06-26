using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Action/Repair Building")]
public class A_RepairBuilding : UtilityAction
{
    [Header("Movement")]
    public float interactDistance = 1.5f;

    [Header("Repair")]
    public int repairAmountPerTick = 10;
    public float repairTickInterval = 0.5f;

    public override bool CanRun(UtilityAgent agent)
    {
        return agent != null && FindBestRepairTarget(agent) != null;
    }

    public override IEnumerator Execute(UtilityAgent agent)
    {
        BuildingAccidentHandler building = FindBestRepairTarget(agent);

        if (building == null)
            yield break;

        if (!building.TryClaimEmergency(agent))
            yield break;

        Debug.Log($"{agent.name} reclama reparación en {building.name}.");

        try
        {
            yield return agent.mover.GoTo(building.GetEmergencyPosition(), interactDistance);
            agent.mover.Stop();

            if (building == null)
                yield break;

            Debug.Log($"{agent.name} empieza a reparar {building.name}.");

            agent.mover.PlayCastingLoop();

            while (building != null && building.NeedsRepair && !building.IsOnFire)
            {
                building.Repair(repairAmountPerTick);
                yield return new WaitForSeconds(repairTickInterval);
            }

            agent.mover.StopCastingLoop();

            if (building != null && !building.NeedsRepair)
                Debug.Log($"{agent.name} ha reparado {building.name}.");
        }
        finally
        {
            agent.mover.StopCastingLoop();

            if (building != null)
                building.ReleaseEmergencyClaim(agent);
        }
    }

    private BuildingAccidentHandler FindBestRepairTarget(UtilityAgent agent)
    {
        BuildingAccidentHandler[] buildings =
            FindObjectsByType<BuildingAccidentHandler>(FindObjectsSortMode.None);

        BuildingAccidentHandler best = null;
        float bestHealth01 = 1f;
        float bestDistance = float.MaxValue;

        foreach (BuildingAccidentHandler building in buildings)
        {
            if (building == null)
                continue;

            if (building.HasEmergencyClaim)
                continue;

            if (building.IsOnFire)
                continue;

            if (!building.NeedsRepair)
                continue;

            float distance = Vector3.Distance(agent.transform.position, building.transform.position);

            bool isWorseHealth = building.Health01 < bestHealth01;
            bool sameHealthButCloser =
                Mathf.Approximately(building.Health01, bestHealth01) &&
                distance < bestDistance;

            if (isWorseHealth || sameHealthButCloser)
            {
                best = building;
                bestHealth01 = building.Health01;
                bestDistance = distance;
            }
        }

        return best;
    }
}