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
        StartCoroutine(VillageLog());
    }
    #region FOOD METHODS
    public bool TryAddFood(int amount)
    {
        int before = village.foodStock;
        village.foodStock = Mathf.Min(village.foodStock + amount, village.foodCapacity); // Mathf.Min por si supera la capacidad
        Debug.Log($"[GameManager] Food {before} -> {village.foodStock} (+{amount})");
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
    #region WOOD METHODS
    public bool TryAddWood(int amount)
    {
        int before = village.woodStock;
        village.woodStock = Mathf.Min(village.woodStock + amount, village.woodCapacity); // Mathf.Min por si supera la capacidad
        Debug.Log($"[GameManager] Wood {before} -> {village.woodStock} (+{amount})");
        if (village.woodStock != before) OnResourceChanged?.Invoke();
        return village.woodStock != before; // Es distinto si se ha agregado comida
    }
    public bool TryConsumeWood(int amount)
    {
        if (village.woodStock < amount) return false;
        village.woodStock -= amount;
        OnResourceChanged?.Invoke();
        return true;
    }
    #endregion

    IEnumerator VillageLog()
    {
        while (true)
        {
            yield return new WaitForSeconds(8);
            Debug.LogError(village.ToString());
        }
    }
}
