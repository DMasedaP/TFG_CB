using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(AgentMover))]
public class UtilityAgent : MonoBehaviour
{
    [Header("Referencias")]
    public Blackboard blackboard;
    public UtilityAction[] actions;
    public UtilityAction eatAction;

    [HideInInspector] public AgentMover mover;

    [Header("Estado del agente")]
    public float decisionInterval = 0.5f;
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

    [Header("Feedback Acciones")]
    [SerializeField] private ActionIconDisplay actionIconDisplay;

    private Coroutine currentAction;
    private float nextDecisionTime;

    private bool isDead = false;

    public Blackboard Blackboard => blackboard;
    public GameManager GameManager => blackboard.resources;
    public VillageState Village => GameManager.village;

    private PopulationManager popManager;

    private void Awake()
    {
        mover = GetComponent<AgentMover>();

        if (eatAction == null)
            Debug.LogError("Asignar accion: A_EatFood");
        if (actionIconDisplay == null)
            actionIconDisplay = GetComponentInChildren<ActionIconDisplay>(true);
    }

    private void Start()
    {
        popManager = FindObjectOfType<PopulationManager>();

        if (blackboard == null)
            blackboard = FindAnyObjectByType<Blackboard>();

        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.OnNightStarted += HandleNightStarted;
            DayNightCycle.Instance.OnDayStarted += HandleDayStarted;
        }
    }

    private void Update()
    {
        if (isDead)
            return;

        hunger01 = Mathf.Clamp01(hunger01 + Time.deltaTime * HUNGER_FACTOR);

        if (hunger01 >= 1f && eatAction != null && currentAction == null)
        {
            SetCurrentActionIcon(eatAction);
            currentAction = StartCoroutine(Run(eatAction));
            hunger01 = 0f;
            return;
        }

        if (Time.time >= nextDecisionTime && currentAction == null)
        {
            nextDecisionTime = Time.time + decisionInterval;
            Decide();
        }
    }

    #region Day Night

    private void HandleNightStarted()
    {
        if (isDead || this == null)
            return;

        sleptOutsideLastNight = false;
        isSleeping = false;

        InterruptAndDecide();
    }

    private void HandleDayStarted()
    {
        if (isDead || this == null)
            return;

        ResolveSleepOutcome();

        isSleeping = false;

        PopulationManager.Instance?.ReleaseHouse(this);
        assignedHouse = null;

        InterruptAndDecide();
    }

    #endregion

    private void Decide()
    {
        if (isDead)
            return;

        if (actions == null || actions.Length == 0)
            return;

        var validActions = actions.Where(a => a != null && a.CanRun(this));

        UtilityAction best = null;
        float bestScore = -1f;

        foreach (UtilityAction action in validActions)
        {
            float score = action.Score(this);

            if (score > bestScore)
            {
                bestScore = score;
                best = action;
            }
        }

        if (best != null)
        {
            SetCurrentActionIcon(best);
            currentAction = StartCoroutine(Run(best));            
        }
    }

    public void CancelCurrentAction()
    {
        if (isDead || this == null)
            return;

        if (currentAction != null)
        {
            StopCoroutine(currentAction);
            currentAction = null;
        }
    }

    public void InterruptAndDecide()
    {
        if (isDead || this == null)
            return;

        CancelCurrentAction();
        ForceDecision();
    }

    public void ForceDecision()
    {
        if (isDead || this == null)
            return;

        nextDecisionTime = Time.time;

        if (currentAction == null)
            Decide();
    }

    private void ResolveSleepOutcome()
    {
        if (sleptOutsideLastNight)
        {
            outdoorSleepCounter++;

            float deathChance = Mathf.Clamp01(0.10f * outdoorSleepCounter);
            float roll = Random.value;

            if (roll < deathChance)
            {
                Debug.LogError($"rnd({roll}) < deathChance({deathChance})");
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
        if (isDead)
            return;

        isDead = true;

        UnsubscribeFromDayNight();

        if (PopulationManager.Instance != null)
            PopulationManager.Instance.ReleaseHouse(this);

        if (currentAction != null)
        {
            StopCoroutine(currentAction);
            currentAction = null;
        }

        Village.citizens = Mathf.Max(0, Village.citizens - 1);

        Destroy(gameObject);
    }

    public void DieFromAccident(string reason)
    {
        Debug.Log($"{name}: {reason}");

        if (isDead)
            return;

        Die();
    }

    private IEnumerator Run(UtilityAction action)
    {
        yield return action.Execute(this);

        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));

        currentAction = null;
    }

    public IEnumerator WaitSeconds(float t)
    {
        yield return new WaitForSeconds(t);
    }

    private void OnDestroy()
    {
        UnsubscribeFromDayNight();

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

    public void SetCurrentActionIcon(UtilityAction action)
    {
        if (actionIconDisplay == null)
            return;

        if (action == null)
        {
            actionIconDisplay.HideIcon();
            return;
        }

        actionIconDisplay.SetIcon(action.actionIcon);
    }
}