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
        BuildingAccidentHandler[] buildings =
            FindObjectsByType<BuildingAccidentHandler>(FindObjectsSortMode.None);

        BuildingAccidentHandler best = null;
        float bestDistance = float.MaxValue;

        foreach (BuildingAccidentHandler building in buildings)
        {
            if (building == null)
                continue;

            if (building.HasEmergencyClaim)
                continue;

            if (!building.IsOnFire)
                continue;

            float distance = Vector3.Distance(agent.transform.position, building.transform.position);

            if (distance < bestDistance)
            {
                best = building;
                bestDistance = distance;
            }
        }

        return best;
    }
}
