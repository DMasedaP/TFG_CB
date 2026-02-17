using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "TFG/US/Consideration/StoneStockLow")]

public class C_StoneStock : UtilityConsideration
{
    // Sube la utilidad de minar Oro cuando el stock está por debajo del objetivo
    protected override float Query(UtilityAgent agent)
    {
        var v = agent.Blackboard.village;
        if (v.targetStoneBuffer <= 0) return 0f;
        float ratio = Mathf.Clamp01((float)v.stoneStock / v.targetStoneBuffer);
        return 1f - ratio;
    }
}
