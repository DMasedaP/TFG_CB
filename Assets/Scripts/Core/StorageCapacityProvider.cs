using System.Collections;
using UnityEngine;
/*
 * Añadir en cada almacen (Granary, Sawmill y OresStorage)
 * Llamar a RegisterCapacity al construir el edificio
 * Llamar a UnregisterCapaity en el OnDestroy()
 */
public class StorageCapacityProvider : MonoBehaviour
{
    [Header("Capacidad que aporta este edificio")]
    public int foodCapacityToAdd;
    public int woodCapacityToAdd;
    public int stoneCapacityToAdd;
    public int goldCapacityToAdd;

    private bool capacityRegistered = false;
    IEnumerator RegisterCapacity()
    {
        yield return new WaitForSeconds(1);
        if (capacityRegistered) yield return null;

        var bb = FindAnyObjectByType<Blackboard>();
        if (bb == null)
        {
            Debug.LogError("No encuentro Blackboard");
            yield return null;
        }
        var villageState = bb.village;
        if(villageState == null)
        {
            Debug.LogError("No encuentro VillageState");
            yield return null;
        }
        villageState.foodCapacity += foodCapacityToAdd;
        villageState.woodCapacity += woodCapacityToAdd;
        villageState.stoneCapacity += stoneCapacityToAdd;
        villageState.goldCapacity += goldCapacityToAdd;

        capacityRegistered = true;

        Debug.Log($"{name} ha añadido capacidad al VillageState.");
    }

    IEnumerator UnregisterCapacity()
    {
        yield return new WaitForSeconds(1);
        if (!capacityRegistered) yield return null;

        var villageState = FindAnyObjectByType<GameManager>().village;
        villageState.foodCapacity -= foodCapacityToAdd;
        villageState.woodCapacity -= woodCapacityToAdd;
        villageState.stoneCapacity -= stoneCapacityToAdd;
        villageState.goldCapacity -= goldCapacityToAdd;

        villageState.foodCapacity = Mathf.Max(0, villageState.foodCapacity);
        villageState.woodCapacity = Mathf.Max(0, villageState.woodCapacity);
        villageState.stoneCapacity = Mathf.Max(0, villageState.stoneCapacity);
        villageState.goldCapacity = Mathf.Max(0, villageState.goldCapacity);

        capacityRegistered = false;

        Debug.Log($"{name} ha quitado capacidad del VillageState.");
    }

    private void OnDestroy()
    {
        StartCoroutine(UnregisterCapacity());
    }
    private void Awake()
    {
        StartCoroutine(RegisterCapacity());
    }
}