using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="TFG/US/Consideration/FoodNeedByPopulation")]
public class C_FoodNeedByPopulation : UtilityConsideration
{
    [Header("Food Balance")]
    [SerializeField] private float foodConsumedPerMeal = 5f;
    [SerializeField] private float secondsToHungry = 150f;

    [Header("Desired Buffer")]
    [SerializeField] private float targetFoodMinutes = 6f;
    [SerializeField] private float criticalFoodMinutes = 2f;

    /// <summary>
    /// Esta consideracion calcula la necesidad segun los minutos de comida disponibles
    /// </summary>
    protected override float Query(UtilityAgent agent)
    {
        var v = agent.Blackboard.village;

        if (v.TotalCitizens <= 0)
            return 0f;

        float foodPerMinutePerCitizen = foodConsumedPerMeal / (secondsToHungry / 60f);
        float foodNeedPerMinute = v.TotalCitizens * foodPerMinutePerCitizen;

        if (foodNeedPerMinute <= 0f)
            return 0f;

        float currentFoodMinutes = v.foodStock / foodNeedPerMinute;

        if (currentFoodMinutes <= criticalFoodMinutes)
            return 1f;

        if (currentFoodMinutes >= targetFoodMinutes)
            return 0f;

        float t = Mathf.InverseLerp(targetFoodMinutes, criticalFoodMinutes, currentFoodMinutes);
        return Mathf.Clamp01(t);
    }
}
