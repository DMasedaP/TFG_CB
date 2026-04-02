using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private Transform sleepPoint;
    private HashSet<UtilityAgent> occupants = new HashSet<UtilityAgent>();

    public int Capacity = 3; 
    public int OccupiedCount => occupants.Count;
    public bool HasFreeSlot => OccupiedCount < Capacity;
    public Vector3 SleepPosition => sleepPoint != null ? sleepPoint.position : transform.position;

    private void Start()
    {
        PopulationManager.Instance?.RegisterHouse(this);
    }
    private void OnDestroy()
    {
        PopulationManager.Instance?.UnregisterHouse(this);
    }

    public bool TryOccupy(UtilityAgent agent)
    {
        if (agent == null) return false;
        if (occupants.Contains(agent)) return true;
        if (!HasFreeSlot) return false;

        occupants.Add(agent);
        return true;
    }
    public void Release(UtilityAgent agent)
    {
        if (agent == null) return;
        occupants.Remove(agent);
    }
}
