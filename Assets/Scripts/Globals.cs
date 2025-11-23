using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public static T FindNearest<T>(Vector3 from, float radius) where T : Component
    {
        var all = Object.FindObjectsOfType<T>();
        float r2 = radius * radius;
        T best = null; float bestD2 = float.MaxValue;
        foreach (var t in all)
        {
            float d2 = (t.transform.position - from).sqrMagnitude;
            if (d2 <= r2 && d2 < bestD2) { bestD2 = d2; best = t; }
        }
        return best;
    }
}
