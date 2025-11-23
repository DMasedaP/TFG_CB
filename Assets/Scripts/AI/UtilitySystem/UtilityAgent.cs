using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AgentMover))]
public class UtilityAgent : MonoBehaviour
{
    [Header("Referencias")]
    public Blackboard blackboard;
    public UtilityAction[] actions;

    [HideInInspector] public AgentMover mover;

    [Header("Estado del agente")]
    public float decisionInterval = 0.5f; // Cada cuanto decide (segs)
    public float hunger01; // 0 = lleno, 1 = hambriento

    [Header("Inventario")]
    public int inventoryFood;
    public int inventoryWood;

    public int maxFood = 10;
    public int maxWood = 1;

    Coroutine currentAction;
    float nextDecisionTime;

    public Blackboard Blackboard => blackboard;
    public GameManager GameManager => blackboard.resources; // Acceso directo
    public VillageState Village => GameManager.village;

    private void Awake()
    {
        mover = GetComponent<AgentMover>();
    }
    private void Update()
    {
        // El hambre sube lentamente con el tiempo
        hunger01 = Mathf.Clamp01(hunger01 + Time.deltaTime * 0.01f);

        // Si no esta ejecutando una accion, decide
        if(Time.time >= nextDecisionTime && currentAction == null)
        {
            nextDecisionTime = Time.time + decisionInterval;
            Decide();
        }
    }

    void Decide()
    {
        // Calcula la puntuacion de cada accion disponible
        var validActions = actions.Where(a => a != null && a.CanRun(this));

        UtilityAction best = null;
        float bestScore = -1f;

        foreach(var a in validActions)
        {
            float score = a.Score(this);
            if(score > bestScore)
            {
                bestScore = score;
                best = a;
            }
        }
        if (best != null)
        {
            currentAction = StartCoroutine(Run(best));
        }
    }

    IEnumerator Run(UtilityAction action)
    {
        yield return action.Execute(this);
        // Pequeña pausa antes de volver a decidir
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        currentAction = null;
    }

    public IEnumerator WaitSeconds(float t)
    {
        yield return new WaitForSeconds(t);
    }
}
