using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentMover : MonoBehaviour
{
    private NavMeshAgent agent;
    private float baseSpeed;
    private float multiplier = 1f;

    private Animator animator;

    [Header("Animation")]
    private string speedParam = "Speed";
    private float animationSmoothTime = 0.5f;

    private float currentAnimationSpeed;

    [Header("Sleep Visuals")]
    [SerializeField] private Transform visualRoot;

    [Header("Sleep Danger Feedback")]
    [SerializeField] private Renderer[] sleepDangerRenderers;
    [SerializeField] private Material originalSleepMaterial;
    [SerializeField] private Material sleepDangerMaterial;
    [SerializeField] private float sleepDangerBlinkInterval = 0.5f;

    private Quaternion originalVisualLocalRotation;
    private Vector3 originalVisualLocalPosition;

    private Coroutine sleepDangerCoroutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        animator = visualRoot.GetComponent<Animator>();
        if (animator == null) Debug.LogError("No hay animator, arregle esto");
        // Visuals para sleep
        originalVisualLocalRotation = visualRoot.localRotation;
        originalVisualLocalPosition = visualRoot.localPosition;
    }

    public IEnumerator GoTo(Vector3 pos, float stoppingDist)
    {
        if (!agent.enabled) agent.enabled = true;
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDist; // Ajustable
        agent.SetDestination(pos);
        while(agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        //Stop();
        //ForceIdleAnimation();
    }
    public void Stop()
    {
        agent.isStopped = true;
    }
    #region SLEEP METHODS
    public void LayDown()
    {
        Stop();
        ForceIdleAnimation();
        agent.ResetPath(); //
        agent.updateRotation = false;
        agent.enabled = false;
        if (visualRoot != null)
        {
            Debug.LogError("LayDown llamado. VisualRoot: " + visualRoot.name);
            visualRoot.localRotation = originalVisualLocalRotation * Quaternion.Euler(90f, 0f, 0f);
        }
    }
    public void StandUp()
    {
        Debug.LogError("STRANDUP");
        StopSleepDangerFeedback();
        if (visualRoot != null)
        {
            visualRoot.localRotation = originalVisualLocalRotation;
            visualRoot.localPosition = originalVisualLocalPosition;
        }
        agent.enabled = true;
        agent.updateRotation = true;
        ForceIdleAnimation();
    }
    public void HideVisual()
    {
        Debug.LogError("Escondo");
        StopSleepDangerFeedback();
        visualRoot.gameObject.SetActive(false);
    }

    public void ShowVisual()
    {
        Debug.LogError("Muestro");
        visualRoot.gameObject.SetActive(true);
    }
    #endregion
    private void Update()
    {
        // Para cuando llueva
        if (NaturalAccidentManager.Instance != null)
            multiplier = NaturalAccidentManager.Instance.CurrentMoveSpeedMultiplier;
        agent.speed = baseSpeed * multiplier;

        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        if (animator == null || agent == null || !agent.enabled)
            return;

        float targetSpeed = agent.velocity.magnitude;

        currentAnimationSpeed = Mathf.Lerp(
            currentAnimationSpeed,
            targetSpeed,
            Time.deltaTime / animationSmoothTime
        );

        animator.SetFloat(speedParam, currentAnimationSpeed);
    }
    private void ForceIdleAnimation()
    {
        if (animator == null)
            return;

        animator.SetFloat(speedParam, 0f);
        animator.Play("Idle", 0, 0f);
        animator.Update(0f);
    }

    // Para los Incendios / Reparaciones
    public void PlayCastingLoop()
    {
        Stop();

        if (animator == null)
            return;

        animator.SetFloat(speedParam, 0f);
        animator.Play("CastingLoop", 0, 0f);
    }

    public void StopCastingLoop()
    {
        ForceIdleAnimation();
    }

    #region Sleep Danger Feedback
    public void StartSleepDangerFeedback()
    {
        StopSleepDangerFeedback();

        if (originalSleepMaterial == null)
        {
            Debug.LogWarning($"{name}: Falta asignar originalSleepMaterial.");
            return;
        }

        if (sleepDangerMaterial == null)
        {
            Debug.LogWarning($"{name}: Falta asignar sleepDangerMaterial.");
            return;
        }

        EnsureSleepDangerRenderers();

        if (sleepDangerRenderers == null || sleepDangerRenderers.Length == 0)
        {
            Debug.LogWarning($"{name}: No se han encontrado renderers para el feedback de dormir fuera.");
            return;
        }

        sleepDangerCoroutine = StartCoroutine(SleepDangerFeedbackCoroutine());
    }

    public void StopSleepDangerFeedback()
    {
        if (sleepDangerCoroutine != null)
        {
            StopCoroutine(sleepDangerCoroutine);
            sleepDangerCoroutine = null;
        }

        SetMaterialToAllSleepRenderers(originalSleepMaterial);
    }

    private IEnumerator SleepDangerFeedbackCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(Mathf.Max(0.05f, sleepDangerBlinkInterval));

        while (true)
        {
            SetMaterialToAllSleepRenderers(sleepDangerMaterial);
            yield return wait;

            SetMaterialToAllSleepRenderers(originalSleepMaterial);
            yield return wait;
        }
    }

    private void EnsureSleepDangerRenderers()
    {
        if (sleepDangerRenderers != null && sleepDangerRenderers.Length > 0)
            return;

        if (visualRoot != null)
            sleepDangerRenderers = visualRoot.GetComponentsInChildren<Renderer>(true);
        else
            sleepDangerRenderers = GetComponentsInChildren<Renderer>(true);
    }

    private void SetMaterialToAllSleepRenderers(Material materialToApply)
    {
        if (sleepDangerRenderers == null)
            return;

        for (int i = 0; i < sleepDangerRenderers.Length; i++)
        {
            if (sleepDangerRenderers[i] == null)
                continue;

            sleepDangerRenderers[i].material = materialToApply;
        }
    }
    #endregion

    private void OnDisable()
    {
        StopSleepDangerFeedback();
    }
}
