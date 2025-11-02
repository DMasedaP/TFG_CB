using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [Header("Asset base (VillageState)")]
    public VillageState villageBase;

    [Header("Instancia runetime (VillageState)")]
    public VillageState village;

    public event System.Action OnResourceChanged;

    private void Awake()
    {
        village = ScriptableObject.Instantiate(villageBase); // Copia en mem del SO base
    }

    #region FOOD METHODS
    public bool TryAddFood(int amount)
    {
        int before = village.foodStock;
        village.foodStock = Mathf.Min(village.foodStock + amount, village.foodCapacity); // Mathf.Min por si supera la capacidad
        if (village.foodStock != before) OnResourceChanged?.Invoke();
        return village.foodStock != before; // Es distinto si se ha agregado comida
    }
    public bool TryConsumeFood(int amount)
    {
        if(village.foodStock < amount) return false;
        village.foodStock -= amount;
        OnResourceChanged?.Invoke();
        return true;
    }
    #endregion
}
