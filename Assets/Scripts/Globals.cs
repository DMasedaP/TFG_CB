using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public static T FindNearestWithFreeSpot<T>(Vector3 from, float radius) where T : Component, IWorkplace
    {
        var all = Object.FindObjectsOfType<T>();
        float r2 = radius * radius;

        T best = null; 
        float bestD2 = float.MaxValue;
        
        foreach (var t in all)
        {
            if (!t.HasFreeSpot()) continue; // Filtro
            float d2 = (t.transform.position - from).sqrMagnitude;
            if (d2 <= r2 && d2 < bestD2) { bestD2 = d2; best = t; }
        }
        return best;
    }

    public static MineSite FindNearestMine(Vector3 from, float radius, MineType type)
    {
        var all = Object.FindObjectsOfType<MineSite>();
        float r2 = radius * radius;
        MineSite best = null; float bestD2 = float.MaxValue;

        foreach (var m in all)
        {
            if (!m.constructed) continue;
            if (m.ResourceId != (type == MineType.Gold ? "Gold" : "Stone")) continue;
            if (!m.HasFreeSpot()) continue;

            float d2 = (m.transform.position - from).sqrMagnitude;
            if (d2 <= r2 && d2 < bestD2) { bestD2 = d2; best = m; }
        }
        return best;
    }
}
