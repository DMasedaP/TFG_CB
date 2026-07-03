using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "TFG/US/Consideration/StoneStockLow")]

public class C_StoneStock : UtilityConsideration
{
    [Header("Urgency")]
    [SerializeField] private float criticalRatio = 0.20f;
    [SerializeField] private float safeRatio = 0.75f;

    protected override float Query(UtilityAgent agent)
    {
        var v = agent.Blackboard.village;

        if (v.targetStoneBuffer <= 0)
            return 0f;

        float ratio = Mathf.Clamp01((float)v.stoneStock / v.targetStoneBuffer);

        if (ratio <= criticalRatio)
            return 1f;

        if (ratio >= safeRatio)
            return 0f;

        float t = Mathf.InverseLerp(safeRatio, criticalRatio, ratio);
        return Mathf.Clamp01(t);
    }
}
