using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
 * Registra las casas construidas
 * Util para saber las plazas disponibles para los aldeanos
 * Actualiza VillageState.maxCitizens
 * Podemos asignar una casa a un aldeano cuando vayan a dormir
 */
public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }
    public GameManager gm;
    [SerializeField]private VillageState villageState;

    [Header("Creacion de civiles")]
    [SerializeField] private GameObject citizenPrefab;
    [SerializeField] private Transform citizenSpawnPoint;
    [SerializeField] private int citizenFoodCost = 10;
    [SerializeField] private float navMeshSpawnSearchRadius = 3;

    private List<House> houses = new();
    private Dictionary<UtilityAgent, House> assignments = new();
    public int TotalHousingCapacity { get; private set; }
    public int OccupiedSlots { get; private set; }
    public int FreeSlots => Mathf.Max(0, TotalHousingCapacity - OccupiedSlots);

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (citizenPrefab == null) Debug.LogError("No se ha asignado el prefab del civil en PopulationManager.");
        if (citizenSpawnPoint == null) Debug.LogError("No se ha asignado un punto de aparición para los civiles.");
    }
    private void Start()
    {
        villageState = gm.village;
        if (villageState == null) Debug.LogError("No se ha asignado villageState [PopulationManager].");
    }
    #region House Management
    public void RegisterHouse(House h)
    {
        houses.Add(h);
        RecalculateHousing();
    }
    public void UnregisterHouse(House h)
    {
        List<UtilityAgent> toRelease = new List<UtilityAgent>();

        foreach (var kvp in assignments)
        {
            if (kvp.Value == h)
                toRelease.Add(kvp.Key);
        }

        foreach (var agent in toRelease)
            assignments.Remove(agent);

        houses.Remove(h);
        RecalculateHousing();
    }
    public void RecalculateHousing()
    {
        int total = 0;
        int occupied = 0;

        foreach (var h in houses)
        {
            if (h == null) continue;
            total += h.Capacity;
            occupied += h.OccupiedCount;
        }

        TotalHousingCapacity = total;
        OccupiedSlots = occupied;

        if (villageState != null)
            villageState.maxCitizens = TotalHousingCapacity;
    }
    public bool TryAssignHouse(UtilityAgent agent, out House house)
    {
        house = null;
        if (agent == null) return false;

        if (assignments.TryGetValue(agent, out var currentHouse) && currentHouse != null)
        {
            house = currentHouse;
            return true;
        }

        foreach (var h in houses)
        {
            if (h == null) continue;

            if (h.TryOccupy(agent))
            {
                assignments[agent] = h;
                house = h;
                RecalculateHousing();
                return true;
            }
        }

        return false;
    }
    public void ReleaseHouse(UtilityAgent agent)
    {
        if (agent == null) return;

        if (!assignments.TryGetValue(agent, out var house))
            return;

        if (house != null)
            house.Release(agent);

        assignments.Remove(agent);
        RecalculateHousing();
    }
    #endregion


    #region Registrar poblacion
    public void RegisterCitizen()   => gm.village.citizens++;
    public void UnregisterCitizen() => gm.village.citizens = Mathf.Max(0, gm.village.citizens - 1);
    public void RegisterArmy()      => gm.village.army++;
    public void UnregisterArmy()    => gm.village.army = Mathf.Max(0, gm.village.army - 1);
    #endregion

    #region Crear Citizen
    public void CreateCitizenBtn()
    {
        // Comprobaqciones
        if (villageState == null) return;
        if (citizenPrefab == null) return;
        if (citizenSpawnPoint == null) return;
        // Para crear un nuevo civil debe de haber la suficiente comida
        if(villageState.foodStock < citizenFoodCost)
        {
            Debug.LogError("No se puede crear un civil: No hay suficiente comida");
            return;
        }
        Vector3 spawnPosition = citizenSpawnPoint.position;
        // Buscamos un punto valido del NavMesh cercano al spawnpoint
        if(NavMesh.SamplePosition(citizenSpawnPoint.position, out NavMeshHit hit, navMeshSpawnSearchRadius, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
        }
        else
        {
            Debug.LogError("No se encontro navmesh cerca dle spawnpoint");
        }
        // Pagamos el coste de generar un civil
        villageState.foodStock -= citizenFoodCost;        
        gm.uiManager.UpdateResources();
        Instantiate(citizenPrefab, spawnPosition, citizenSpawnPoint.rotation);
    }
    #endregion
}
