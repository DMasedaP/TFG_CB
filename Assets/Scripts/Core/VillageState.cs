using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="TFG/VillageState")]
public class VillageState : ScriptableObject
{
    [Header("Food")]
    public int foodStock;
    public int foodCapacity;

    [Header("Population")]
    public int citizens;    // Campesinos
    public int army;        // Ejercito
    public int enemies;     // Enemigos

    [Header("Balance")]
    public float foodPerCitizenPerDay = 1.0f;   // Configurable
    public float ticksPerDay = 24f;             // 24h del juego

    public int TotalCitizens => citizens + army;
    public float FoodPerTick => (TotalCitizens * foodPerCitizenPerDay) / Mathf.Max(1f, ticksPerDay);

    public override string ToString()
    {
        return $"--- POPULATION ---\n" +
            $"Citizens: {citizens}\n" +
            $"Army: {army}\n" +
            $"Enemies: {enemies}\n" +
            $"--- FOOD ---\n" +
            $"Stock: {foodStock}\n" +
            $"Capacity: {foodCapacity}\n";
    }
}
