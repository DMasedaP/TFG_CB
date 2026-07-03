using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "TFG/US/Consideration/WoodStockLow")]
public class C_WoodStock : UtilityConsideration
{
    [Header("Urgency")]
    [SerializeField] private float criticalRatio = 0.25f;
    [SerializeField] private float safeRatio = 0.85f;

    protected override float Query(UtilityAgent agent)
    {
        var v = agent.Blackboard.village;

        if (v.targetWoodBuffer <= 0)
            return 0f;

        float ratio = Mathf.Clamp01((float)v.woodStock / v.targetWoodBuffer);

        if (ratio <= criticalRatio)
            return 1f;

        if (ratio >= safeRatio)
            return 0f;

        float t = Mathf.InverseLerp(safeRatio, criticalRatio, ratio);
        return Mathf.Clamp01(t);
    }
}
