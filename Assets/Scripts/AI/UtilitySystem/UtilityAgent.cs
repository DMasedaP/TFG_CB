using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AgentMover))]
public class UtilityAgent : MonoBehaviour
{
    [Header("Referencias")]
    public Blackboard blackboard;
    public UtilityAction[] actions;
    public UtilityAction eatAction;

    [HideInInspector] public AgentMover mover;

    [Header("Estado del agente")]
    public float decisionInterval = 0.5f; // Cada cuanto decide (segs)
    public float hunger01; // 0 = lleno, 1 = hambriento
    private const float HUNGER_FACTOR = 0.02f;

    [Header("Inventario")]
    public int inventoryFood;
    public int inventoryWood;
    public int inventoryStone;
    public int inventoryGold;

    public int maxFood = 10;
    public int maxWood = 1;
    public int maxStone = 1;
    public int maxGold = 2;

    [Header("Sueño")]
    public bool isSleeping;
    public bool sleptOutsideLastNight;
    public House assignedHouse;
    public int outdoorSleepCounter = 0;

    Coroutine currentAction;
    float nextDecisionTime;

    private bool isDead = false;

    public Blackboard Blackboard => blackboard;
    public GameManager GameManager => blackboard.resources; // Acceso directo
    public VillageState Village => GameManager.village;
    private PopulationManager popManager;

    private void Awake()
    {
        mover = GetComponent<AgentMover>();
        if (eatAction == null) Debug.LogError("Asignar accion: A_EatFood");
    }
    private void Start()
    {
        popManager = FindObjectOfType<PopulationManager>();
        if(Blackboard == null) blackboard = FindAnyObjectByType<Blackboard>();
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.OnNightStarted += HandleNightStarted;
            DayNightCycle.Instance.OnDayStarted += HandleDayStarted;
        }
    }
    private void Update()
    {
        // El hambre sube lentamente con el tiempo
        hunger01 = Mathf.Clamp01(hunger01 + Time.deltaTime * HUNGER_FACTOR);

        if (hunger01 == 1f)
        {
            StartCoroutine(Run(eatAction));
            hunger01 = 0; // Reseteamos
        }
        // Si no esta ejecutando una accion, decide
        if(Time.time >= nextDecisionTime && currentAction == null)
        {
            nextDecisionTime = Time.time + decisionInterval;
            Decide();
        }
    }
    #region Day Night
    private void HandleNightStarted()
    {
        if (isDead || this == null) return;
        sleptOutsideLastNight = false;
        isSleeping = false;
        InterruptAndDecide();
    }

    private void HandleDayStarted()
    {
        if (isDead || this == null) return;
        ResolveSleepOutcome();
        isSleeping = false;
        PopulationManager.Instance?.ReleaseHouse(this);
        assignedHouse = null;
        InterruptAndDecide();
    }
    #endregion
    void Decide()
    {
        // Calcula la puntuacion de cada accion disponible
        var validActions = actions.Where(a => a != null && a.CanRun(this));        

        UtilityAction best = null;
        float bestScore = -1f;

        foreach(var a in validActions)
        {
            float score = a.Score(this);
            if(score > bestScore)
            {
                bestScore = score;
                best = a;
            }
        }
        if (best != null)
        {
            Debug.Log("Hay mejor accion, inicio coroutine");
            currentAction = StartCoroutine(Run(best));
        }
    }
    public void CancelCurrentAction()
    {
        if (isDead || this == null) return;
        if (currentAction != null)
        {
            StopCoroutine(currentAction);
            currentAction = null;
        }
    }

    public void InterruptAndDecide()
    {
        if (isDead || this == null) return;
        CancelCurrentAction();
        ForceDecision();
    }
    public void ForceDecision()
    {
        nextDecisionTime = Time.time;
        if (currentAction == null)
            Decide();
    }
    /// <summary>
    /// Gestiona si ha dormido fuera una noche
    /// </summary>
    private void ResolveSleepOutcome()
    {
        if (sleptOutsideLastNight)
        {
            outdoorSleepCounter++;

            float deathChance = Mathf.Clamp01(0.10f * outdoorSleepCounter);
            if (Random.value < deathChance)
            {
                Debug.LogError($"rnd({Random.value}) < deathChance({deathChance})");
                Die();
                return;
            }
        }
        else
        {
            outdoorSleepCounter = 0;
        }
    }
    private void Die()
    {
        /*
        isDead = true;
        UnsubscribeFromDayNight();
        Village.citizens = Mathf.Max(0, Village.citizens - 1);
        PopulationManager.Instance?.ReleaseHouse(this);
        Destroy(gameObject); // Ya con esto lanzamos desde CitizenAgent una orden a PopulaitonManager de que quite haga UnRegister
        */
        if (isDead) return;
        isDead = true;

        UnsubscribeFromDayNight();

        if (PopulationManager.Instance != null)
            PopulationManager.Instance.ReleaseHouse(this);

        if (currentAction != null)
        {
            try
            {
                StopCoroutine(currentAction);
            }
            catch
            {
            }

            currentAction = null;
        }

        Village.citizens = Mathf.Max(0, Village.citizens - 1);

        Destroy(gameObject);
    }
    public void DieFromAccident(string reason)
    {
        Debug.Log($"{name}: {reason}");
        if (isDead) return;
        Die();
    }

    IEnumerator Run(UtilityAction action)
    {
        yield return action.Execute(this);
        // Pequeña pausa antes de volver a decidir
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        currentAction = null;
    }

    public IEnumerator WaitSeconds(float t)
    {
        yield return new WaitForSeconds(t);
    }

    private void OnDestroy()
    {
        if (PopulationManager.Instance != null)
            PopulationManager.Instance.ReleaseHouse(this);
    }
    private void UnsubscribeFromDayNight()
    {
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.OnNightStarted -= HandleNightStarted;
            DayNightCycle.Instance.OnDayStarted -= HandleDayStarted;
        }
    }
}
