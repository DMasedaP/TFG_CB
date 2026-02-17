using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="TFG/VillageState")]
public class VillageState : ScriptableObject
{
    [Header("Food")]
    public int foodStock;
    public int foodCapacity; // 50
    
    [Header("Wood")]
    public int woodStock;
    public int woodCapacity; //50
    public int targetWoodBuffer; // 5

    [Header("Stone")]
    public int stoneStock;
    public int stoneCapacity; // 16
    public int targetStoneBuffer; // 6

    [Header("Gold")]
    public int goldStock;
    public int goldCapacity; // 100
    public int targetGoldBuffer; // 20


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
            $"Capacity: {foodCapacity}\n"+
            $"--- WOOD ---\n" +
            $"Stock: {woodStock}\n" +
            $"Capacity: {woodCapacity}\n";
    }
}
