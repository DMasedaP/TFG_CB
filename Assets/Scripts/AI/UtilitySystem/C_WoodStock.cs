using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(menuName = "TFG/US/Consideration/WoodStockLow")]
public class C_WoodStock : UtilityConsideration
{
    // Sube la utilidad de recolectar madera cuando el stock está por debajo del objetivo

    protected override float Query(UtilityAgent agent)
    {
        var v = agent.blackboard.village;
        if(v.targetWoodBuffer <= 0) return 0f; // Sin objetivo, no hay urgencia
        float ratio = Mathf.Clamp01((float)v.woodStock / v.targetWoodBuffer);
        return 1f - ratio; // 1 = mucha necesidad, 0 = cubierto
    }
}
