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
        agent.stoppingDistance = stoppingDist; // Ajustable
        agent.SetDestination(pos);
        while(agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;
    }
}
