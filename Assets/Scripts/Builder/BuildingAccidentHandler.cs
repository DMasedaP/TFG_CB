using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BuildingAccidentHandler : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Fire")]
    [SerializeField] private bool canBurn = true;
    [SerializeField] private GameObject fireVisual;

    [Header("Emergency")]
    [SerializeField] private Transform emergencyPoint;
    private float navMeshSampleRadius = 3f;
    private float reigniteBlockTimeAfterExtinguish = 10f;

    [Header("Decay")]
    [SerializeField] private float decayTickInterval = 10f;
    [SerializeField] private int decayDamagePerTick = 1;
    [SerializeField] private float repairThreshold01 = 0.5f;

    //public bool NeedsRepair => Health01 <= repairThreshold01;
    private Coroutine decayRoutine;

    public event Action<float> OnHealthChanged;
    public float Health01 => maxHealth <= 0 ? 0f : (float)currentHealth / maxHealth; // La vida del [0 - 1]

    public bool IsOnFire { get; private set; }
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool NeedsRepair => currentHealth < maxHealth || Health01 <= repairThreshold01;
    public bool HasEmergencyClaim => emergencyAgent != null;

    private Coroutine fireRoutine;
    private UtilityAgent emergencyAgent;

    private float nextAllowedIgniteTime;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(Health01);

        if (fireVisual != null)
            fireVisual.SetActive(false);
        if (emergencyPoint == null) Debug.LogError("¡Asigna Emergency Point!");
    }
    private void Start()
    {
        decayRoutine = StartCoroutine(BuildingDecay());
    }

    public Vector3 GetEmergencyPosition()
    {
        Vector3 desiredPosition = emergencyPoint != null
            ? emergencyPoint.position
            : transform.position;

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
            return hit.position;

        return desiredPosition;
    }

    public void TryIgnite(float chance, int damagePerTick, float tickInterval)
    {
        if (!canBurn || IsOnFire)
            return;

        if (HasEmergencyClaim)
            return;

        if (Time.time < nextAllowedIgniteTime)
            return;

        if (UnityEngine.Random.value > chance)
            return;

        StartFire(damagePerTick, tickInterval);
    }

    public void StartFire(int damagePerTick, float tickInterval)
    {
        if (IsOnFire)
            return;

        IsOnFire = true;

        if (fireVisual != null)
            fireVisual.SetActive(true);

        Debug.Log($"Incendio iniciado en {name}.");

        fireRoutine = StartCoroutine(FireDamageRoutine(damagePerTick, tickInterval));

        NotifyAgentsToReconsiderActions();
    }

    private IEnumerator FireDamageRoutine(int damagePerTick, float tickInterval)
    {
        while (IsOnFire)
        {
            TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
    }

    public bool TryClaimEmergency(UtilityAgent agent)
    {
        if (agent == null)
            return false;

        if (emergencyAgent != null && emergencyAgent != agent)
            return false;

        if (!IsOnFire && !NeedsRepair)
            return false;

        emergencyAgent = agent;
        return true;
    }

    public void ReleaseEmergencyClaim(UtilityAgent agent)
    {
        if (emergencyAgent == agent)
            emergencyAgent = null;
    }

    public void ExtinguishFire()
    {
        if (!IsOnFire)
            return;

        IsOnFire = false;

        nextAllowedIgniteTime = Time.time + reigniteBlockTimeAfterExtinguish;

        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }

        if (fireVisual != null)
            fireVisual.SetActive(false);

        Debug.Log($"Incendio apagado en {name}.");
    }

    public void Repair(int amount)
    {
        if (amount <= 0)
            return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(Health01);
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        Debug.LogError($"{name} vida: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(Health01);

        if (currentHealth <= 0)
            DestroyBuilding();
    }

    private void DestroyBuilding()
    {
        IsOnFire = false;
        emergencyAgent = null;

        if (fireRoutine != null)
            StopCoroutine(fireRoutine);

        if (fireVisual != null)
            fireVisual.SetActive(false);

        Destroy(gameObject);
    }

    private void NotifyAgentsToReconsiderActions()
    {
        UtilityAgent[] agents = FindObjectsByType<UtilityAgent>(FindObjectsSortMode.None);

        foreach (UtilityAgent agent in agents)
        {
            if (agent == null)
                continue;

            agent.InterruptAndDecide();
        }
    }

    // - - - D E C A Y - - - - - - - - - - -
    private IEnumerator BuildingDecay()
    {
        while (true)
        {
            yield return new WaitForSeconds(decayTickInterval);
            if (!IsOnFire)
                TakeDamage(decayDamagePerTick);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}