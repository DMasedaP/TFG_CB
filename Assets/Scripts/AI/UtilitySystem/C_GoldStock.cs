using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Consideration/GoldStockLow")]
public class C_GoldStock : UtilityConsideration
{
    [Header("Urgency")]
    [SerializeField] private float criticalRatio = 0.15f;
    [SerializeField] private float safeRatio = 0.65f;

    protected override float Query(UtilityAgent agent)
    {
        var v = agent.Blackboard.village;

        if (v.targetGoldBuffer <= 0)
            return 0f;

        float ratio = Mathf.Clamp01((float)v.goldStock / v.targetGoldBuffer);

        if (ratio <= criticalRatio)
            return 1f;

        if (ratio >= safeRatio)
            return 0f;

        float t = Mathf.InverseLerp(safeRatio, criticalRatio, ratio);
        return Mathf.Clamp01(t);
    }
}
