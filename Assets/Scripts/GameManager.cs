using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Asset base (VillageState)")]
    public VillageState villageBase;

    [Header("Instancia runetime (VillageState)")]
    public VillageState villageRuntime;

    private void Awake()
    {
        villageRuntime = ScriptableObject.Instantiate(villageBase); // Copia en mem del SO base
    }
}
