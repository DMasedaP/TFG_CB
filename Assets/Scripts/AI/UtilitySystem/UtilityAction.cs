using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UtilityAction : ScriptableObject
{
    [Header("Meta")]
    public string actionName = "Unnamed";
    [Range(0f, 5f)] public float weight = 1f;

    [Header("Consideraciones")]
    public UtilityConsideration[] considerations;

    /// <summary>
    /// Devuelve la puntuacion (0.. infinito) de la accion para este agente.
    /// Producto de consideraciones (0..1) normalizado y ponderado por weight.
    /// </summary>
    public float Score(UtilityAgent agent)
    {
        if (considerations == null || considerations.Length == 0) return weight;

        float prod = 1f;
        for(int i = 0; i < considerations.Length; i++)
            prod *= Mathf.Clamp01(considerations[i].Evaluate(agent));

        // Normalizacion del producto: 1 - (1 - x)^(1/n)
        float n = Mathf.Max(1, considerations.Length);
        float normalized = 1f - Mathf.Pow(1f - prod, 1f / n);

        return weight * normalized;
    }
    /// <summary>
    /// Comprobacion ligera previa (inventario lleno, no hay destino, etc.)
    /// </summary>
    public virtual bool CanRun(UtilityAgent agent) => true;
    /// <summary>
    /// Logica de la accion (moverse, trabajar, depositar...). Debe terminar sola.
    /// </summary>
    public abstract IEnumerator Execute(UtilityAgent agent);
}
