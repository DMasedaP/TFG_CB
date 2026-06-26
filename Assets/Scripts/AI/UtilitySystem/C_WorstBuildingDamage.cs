using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Consideration/WorstBuildingDamage")]
public class C_WorstBuildingDamage : UtilityConsideration
{
    [Header("Repair")]
    [SerializeField, Range(0f, 1f)] private float repairThreshold01 = 0.5f;

    protected override float Query(UtilityAgent agent)
    {
        BuildingAccidentHandler[] buildings =
            FindObjectsByType<BuildingAccidentHandler>(FindObjectsSortMode.None);

        float worstDamage01 = 0f;

        foreach (BuildingAccidentHandler building in buildings)
        {
            if (building == null)
                continue;

            if (building.HasEmergencyClaim)
                continue;

            if (building.IsOnFire)
                continue;

            if (building.Health01 >= repairThreshold01)
                continue;

            float damage01 = 1f - building.Health01;

            if (damage01 > worstDamage01)
                worstDamage01 = damage01;
        }

        return Mathf.Clamp01(worstDamage01);
    }
}
