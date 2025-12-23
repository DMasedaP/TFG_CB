using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorkplace
{
    bool TryReserveSpot(GameObject worker, out Transform spot);
    void ReleaseSpot(GameObject worker);
    float WorkSeconds { get; }
    bool HasFreeSpot();
}
