using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BASE PARA CONSIDERACIONES
public abstract class UtilityConsideration : ScriptableObject
{
    [SerializeField] private AnimationCurve responseCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public float Evaluate(UtilityAgent agent)
    {
        float raw = Query(agent);
        return Mathf.Clamp01(responseCurve.Evaluate(Mathf.Clamp01(raw)));
    }

    protected abstract float Query(UtilityAgent agent);
}
