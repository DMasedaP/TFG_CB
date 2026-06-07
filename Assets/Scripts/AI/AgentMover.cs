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

    private Quaternion originalVisualLocalRotation;
    private Vector3 originalVisualLocalPosition;

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
}
