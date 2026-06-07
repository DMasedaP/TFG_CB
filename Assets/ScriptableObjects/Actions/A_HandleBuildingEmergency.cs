using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Action/Handle Building Emergency")]
public class A_HandleBuildingEmergency : UtilityAction
{
    [Header("Movement")]
    public float interactDistance = 1.5f;

    [Header("Extinguish")]
    public float extinguishDuration = 5f;
    public float extinguishTickInterval = 0.5f;

    [Range(0f, 1f)]
    public float deathChancePerExtinguishTick = 0.03f;

    [Header("Repair")]
    public int repairAmountPerTick = 10;
    public float repairTickInterval = 0.5f;

    [Header("Utility")]
    public float fireScore = 1000f;
    public float repairScore = 700f;

    public override bool CanRun(UtilityAgent agent)
    {
        return agent != null && FindBestTarget(agent) != null;
    }
    public override IEnumerator Execute(UtilityAgent agent)
    {
        BuildingAccidentHandler building = FindBestTarget(agent);

        if (building == null)
            yield break;

        if (!building.TryClaimEmergency(agent))
            yield break;

        Debug.Log($"{agent.name} reclama emergencia en {building.name}.");

        bool releasedClaim = false;
        
        try
        {
            yield return agent.mover.GoTo(building.GetEmergencyPosition(), interactDistance);
            agent.mover.Stop();

            if (building == null)
                yield break;

            // 1) Apagar incendio. La muerte solo puede ocurrir aquí.
            if (building.IsOnFire)
            {
                Debug.Log($"{agent.name} empieza a apagar el incendio en {building.name}.");

                agent.mover.PlayCastingLoop();

                float elapsed = 0f;

                while (building != null && building.IsOnFire && elapsed < extinguishDuration)
                {
                    if (Random.value <= deathChancePerExtinguishTick)
                    {
                        Debug.Log($"{agent.name} muere intentando apagar el incendio en {building.name}.");

                        building.ReleaseEmergencyClaim(agent);
                        releasedClaim = true;

                        agent.DieFromAccident("Murió intentando apagar un incendio.");
                        yield break;
                    }

                    elapsed += extinguishTickInterval;
                    yield return new WaitForSeconds(extinguishTickInterval);
                }

                agent.mover.StopCastingLoop();

                if (building == null)
                    yield break;

                building.ExtinguishFire();

                Debug.Log($"{agent.name} ha apagado el incendio en {building.name}.");
            }

            // 2) Reparar. Aquí NO hay probabilidad de muerte.
            if (building != null && building.NeedsRepair)
            {
                Debug.Log($"{agent.name} empieza a reparar {building.name}.");

                agent.mover.PlayCastingLoop();

                while (building != null && building.NeedsRepair)
                {
                    building.Repair(repairAmountPerTick);
                    yield return new WaitForSeconds(repairTickInterval);
                }

                agent.mover.StopCastingLoop();
            }

            if (building != null)
                Debug.Log($"{agent.name} ha reparado {building.name} al 100%.");
        }
        finally
        {
            agent.mover.StopCastingLoop(); // Cortamos animacion
            if (!releasedClaim && building != null)
                building.ReleaseEmergencyClaim(agent);
        }
    }

    private BuildingAccidentHandler FindBestTarget(UtilityAgent agent)
    {
        BuildingAccidentHandler[] buildings = FindObjectsByType<BuildingAccidentHandler>(FindObjectsSortMode.None);

        BuildingAccidentHandler best = null;
        float bestPriority = float.MinValue;
        float bestDistance = float.MaxValue;

        foreach (BuildingAccidentHandler building in buildings)
        {
            if (building == null)
                continue;

            if (building.HasEmergencyClaim)
                continue;

            bool isValidEmergency = building.IsOnFire || building.NeedsRepair;

            if (!isValidEmergency)
                continue;

            float priority = building.IsOnFire ? 2f : 1f;
            float distance = Vector3.Distance(agent.transform.position, building.transform.position);

            if (priority > bestPriority ||
                (Mathf.Approximately(priority, bestPriority) && distance < bestDistance))
            {
                best = building;
                bestPriority = priority;
                bestDistance = distance;
            }
        }

        return best;
    }
}
