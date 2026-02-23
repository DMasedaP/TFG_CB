using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [Header("Asset base (VillageState)")]
    public VillageState villageBase;

    [Header("Instancia runetime (VillageState)")]
    public VillageState village;

    public event System.Action OnResourceChanged;

    // UI MANAGER
    private UI_Manager uiManager;

    private void Awake()
    {
        village = ScriptableObject.Instantiate(villageBase); // Copia en mem del SO base     
        uiManager = FindAnyObjectByType<UI_Manager>();
        StartCoroutine(VillageLog());
    }
    #region FOOD METHODS
    public bool TryAddFood(int amount)
    {
        int before = village.foodStock;
        village.foodStock = Mathf.Min(village.foodStock + amount, village.foodCapacity); // Mathf.Min por si supera la capacidad
        Debug.Log($"[GameManager] Food {before} -> {village.foodStock} (+{amount})");
        if (village.foodStock != before) { 
            OnResourceChanged?.Invoke();
            uiManager.UpdateResources(); // Actualizamos la UI
        }
        return village.foodStock != before; // Es distinto si se ha agregado comida
    }
    public bool TryConsumeFood(int amount)
    {
        if(village.foodStock < amount) return false;
        village.foodStock -= amount;
        OnResourceChanged?.Invoke();
        uiManager.UpdateResources(); // Actualizamos la UI
        return true;
    }
    #endregion
    #region WOOD METHODS
    public bool TryAddWood(int amount)
    {
        int before = village.woodStock;
        village.woodStock = Mathf.Min(village.woodStock + amount, village.woodCapacity); // Mathf.Min por si supera la capacidad
        Debug.Log($"[GameManager] Wood {before} -> {village.woodStock} (+{amount})");
        if (village.woodStock != before) { 
            OnResourceChanged?.Invoke();
            uiManager.UpdateResources(); // Actualizamos la UI
        }
        return village.woodStock != before; // Es distinto si se ha agregado comida
    }
    public bool TryConsumeWood(int amount)
    {
        if (village.woodStock < amount) return false;
        village.woodStock -= amount;
        OnResourceChanged?.Invoke();
        uiManager.UpdateResources(); // Actualizamos la UI
        return true;
    }
    #endregion
    #region STONE METHODS
    public bool TryAddStone(int amount)
    {
        if(amount <= 0) return false;
        int before = village.stoneStock;
        village.stoneStock = Mathf.Min(village.stoneStock + amount, village.stoneCapacity);

        if (village.stoneStock != before) // Ha habido un cambio
        {
            OnResourceChanged?.Invoke();
            uiManager.UpdateResources();
            return true;
        }
        else return false;                
    }
    public bool TryConsumeStone(int amount)
    {
        if (village.stoneStock < amount) return false;
        village.stoneStock -= amount;
        OnResourceChanged?.Invoke();
        uiManager.UpdateResources(); // Actualizamos la UI
        return true;
    }
    #endregion

    #region GOLD METHODS
    public bool TryAddGold(int amount)
    {
        if (amount <= 0) return false;
        int before = village.goldStock;
        village.goldStock = Mathf.Min(village.goldStock + amount, village.goldCapacity);

        if (village.goldStock != before) // Ha habido un cambio
        {
            OnResourceChanged?.Invoke();
            uiManager.UpdateResources();
            return true;
        }
        else return false;
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
