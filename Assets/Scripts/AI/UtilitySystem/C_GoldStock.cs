using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Consideration/GoldStockLow")]
public class C_GoldStock : UtilityConsideration
{
    // Sube la utilidad de minar Oro cuando el stock está por debajo del objetivo
    protected override float Query(UtilityAgent agent)
    {
        var v = agent.Blackboard.village;
        if (v.targetGoldBuffer <= 0) return 0f;
        float ratio = Mathf.Clamp01((float)v.goldStock / v.targetGoldBuffer);
        return 1f - ratio;
    }
}
