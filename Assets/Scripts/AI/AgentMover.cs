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
    private float animationSmoothTime = 0.1f;

    private float currentAnimationSpeed;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("No hay animator, arregle esto");
    }

    public IEnumerator GoTo(Vector3 pos, float stoppingDist)
    {
        if (!agent.enabled) agent.enabled = true;
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDist; // Ajustable
        agent.SetDestination(pos);
        while(agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;
    }
    public void Stop()
    {
        agent.isStopped = true;
    }
    public void LayDown()
    {
        Stop();
        agent.updateRotation = false;
        agent.enabled = false;        
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
    public void StandUp()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        agent.enabled = true;
        agent.updateRotation = true;        
    }

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
}
