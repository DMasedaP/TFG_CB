using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentMover : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake() => agent = GetComponent<NavMeshAgent>();

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
}
