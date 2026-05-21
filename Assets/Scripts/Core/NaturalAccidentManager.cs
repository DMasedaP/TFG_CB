using System.Collections;
using UnityEngine;

public enum NaturalAccidentType
{
    Rain,
    ExtremeHeat,
    Flood,
    Storm
}

[System.Serializable]
public class NaturalAccidentConfig
{
    public NaturalAccidentType type;
    public bool enabled = true;

    [Range(0f, 100f)]
    public float probabilityWeight = 1f;

    public float minDuration = 30f;
    public float maxDuration = 60f;
}

public class NaturalAccidentManager : MonoBehaviour
{
    public static NaturalAccidentManager Instance { get; private set; }

    [Header("General")]
    [SerializeField] private float checkInterval = 60f;
    [SerializeField, Range(0f, 1f)] private float accidentChancePerCheck = 0.25f;
    [SerializeField] private NaturalAccidentConfig[] accidents;

    [Header("Rain")]
    [SerializeField] private GameObject rainVisual;
    [SerializeField] private float rainLightMultiplier = 0.45f; // Modificador luz
    [SerializeField] private float rainMoveSpeedMultiplier = 0.65f; // Modificador velocidad agentes

    [Header("Extreme Heat")]
    [SerializeField] private float heatLightMult = 1.25f; // Modificador luz
    [SerializeField] private float heatMoveSpeedMult = 0.85f; // Modificador velocidad agentes
    [SerializeField, Range(0f, 1f)] private float heatDeathChancePerTick = 0.05f; // Prob muerte por ExtremeHeat
    [SerializeField, Range(0f, 1f)] private float buildingIgniteChancePerTick = 0.08f; // Prob incendio por ExtremeHeat
    [SerializeField] private float heatEffectTickInterval = 5f;
    [SerializeField] private int fireDamagePerTick = 10;

    [Header("Lighting")]
    [SerializeField] private DayNightCycle dayNightLighting;

    public bool IsAccidentActive { get; private set; }
    public NaturalAccidentType CurrentAccident { get; private set; }

    public float CurrentMoveSpeedMultiplier { get; private set; } = 1f;

    private float timer;

    private Coroutine accidentRoutine;
    private Coroutine heatRoutine; // Controla los efectos repetidos internos del calor extremo

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (rainVisual != null)
            rainVisual.SetActive(false);
    }

    private void Update()
    {
        if (IsAccidentActive)
            return;

        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            TryTriggerAccident();
        }
    }

    private void TryTriggerAccident()
    {
        if (Random.value > accidentChancePerCheck)
            return;

        NaturalAccidentConfig selected = GetRandomAccident();

        if (selected == null)
            return;

        float duration = Random.Range(selected.minDuration, selected.maxDuration);
        accidentRoutine = StartCoroutine(RunAccident(selected.type, duration));
    }

    private NaturalAccidentConfig GetRandomAccident()
    {
        float totalWeight = 0f;

        foreach (var accident in accidents)
        {
            if (accident != null && accident.enabled)
                totalWeight += accident.probabilityWeight;
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.Range(0f, totalWeight);

        foreach (var accident in accidents)
        {
            if (accident == null || !accident.enabled)
                continue;

            if (roll <= accident.probabilityWeight)
                return accident;

            roll -= accident.probabilityWeight;
        }

        return null;
    }

    private IEnumerator RunAccident(NaturalAccidentType type, float duration)
    {
        IsAccidentActive = true;
        CurrentAccident = type;

        StartAccident(type);

        yield return new WaitForSeconds(duration);

        EndAccident(type);

        IsAccidentActive = false;
        accidentRoutine = null;
    }

    private void StartAccident(NaturalAccidentType type)
    {
        switch (type)
        {
            case NaturalAccidentType.Rain:
                StartRain();
                break;

            case NaturalAccidentType.ExtremeHeat:
                StartExtremeHeat();
                break;

            case NaturalAccidentType.Flood:
                Debug.Log("Inundación sin implementar");
                break;

            case NaturalAccidentType.Storm:
                Debug.Log("Tormenta sin implementar");
                break;
        }
    }

    private void EndAccident(NaturalAccidentType type)
    {
        switch (type)
        {
            case NaturalAccidentType.Rain:
                EndRain();
                break;

            case NaturalAccidentType.ExtremeHeat:
                EndExtremeHeat();
                break;
        }
    }
    #region RAIN
    private void StartRain()
    {
        Debug.Log("Empieza la lluvia");

        if (rainVisual != null)
            rainVisual.SetActive(true);

        CurrentMoveSpeedMultiplier = rainMoveSpeedMultiplier;

        if (dayNightLighting != null)
            dayNightLighting.WeatherLightMultiplier = rainLightMultiplier;
    }

    private void EndRain()
    {
        Debug.Log("Termina la lluvia");

        if (rainVisual != null)
            rainVisual.SetActive(false);

        CurrentMoveSpeedMultiplier = 1f;

        if (dayNightLighting != null)
            dayNightLighting.WeatherLightMultiplier = 1f;
    }
    #endregion
    #region EXTREME HEAT
    private void StartExtremeHeat()
    {
        Debug.Log("Empieza ola de calor extremo");
        CurrentMoveSpeedMultiplier = heatMoveSpeedMult;

        dayNightLighting.WeatherLightMultiplier = heatLightMult;

        heatRoutine = StartCoroutine(ExtremeHeatEffects());
    }
    private void EndExtremeHeat()
    {
        Debug.Log("Termina la ola de calor extremo");
        CurrentMoveSpeedMultiplier = 1f;
        dayNightLighting.WeatherLightMultiplier = 1f;

        if (heatRoutine != null)
        {
            StopCoroutine(heatRoutine);
            heatRoutine = null;
        }
    }
    private IEnumerator ExtremeHeatEffects()
    {
        while (IsAccidentActive && CurrentAccident == NaturalAccidentType.ExtremeHeat)
        {
            TryKillCitizensByHeat();
            TryIgniteBuildings();

            yield return new WaitForSeconds(heatEffectTickInterval);
        }
    }
    private void TryKillCitizensByHeat()
    {
        UtilityAgent[] agents = FindObjectsByType<UtilityAgent>(FindObjectsSortMode.None);

        foreach (UtilityAgent agent in agents)
        {
            if (agent == null)
                continue;

            if (Random.value <= heatDeathChancePerTick)
            {
                agent.DieFromAccident("Murió por una insolación.");
            }
        }
    }
    private void TryIgniteBuildings()
    {
        BuildingAccidentHandler[] buildings =
            FindObjectsByType<BuildingAccidentHandler>(FindObjectsSortMode.None);

        foreach (BuildingAccidentHandler building in buildings)
        {
            if (building == null)
                continue;

            building.TryIgnite(
                buildingIgniteChancePerTick,
                fireDamagePerTick,
                heatEffectTickInterval
            );
        }
    }
    #endregion
}