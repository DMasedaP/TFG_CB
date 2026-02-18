using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="TFG/US/Consideration/FoodNeedByPopulation")]
public class C_FoodNeedByPopulation : UtilityConsideration
{
    protected override float Query(UtilityAgent agent)
    {
        var v = agent.Blackboard.village;
        float dailyNeed = v.TotalCitizens * v.foodPerCitizenPerDay;
        if (dailyNeed <= 0f) return 0f;
        
        float ratio = v.foodStock / dailyNeed; // Stock en 'dias' de comida
        return Mathf.Clamp01(1f - Mathf.Clamp01(ratio));
        // 0 => comida suficiente, 1 => urgencia por producir
    }
}
