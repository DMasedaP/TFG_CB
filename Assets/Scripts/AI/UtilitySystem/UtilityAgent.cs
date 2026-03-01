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
    public UtilityAction eatAction;

    [HideInInspector] public AgentMover mover;

    [Header("Estado del agente")]
    public float decisionInterval = 0.5f; // Cada cuanto decide (segs)
    public float hunger01; // 0 = lleno, 1 = hambriento
    private const float HUNGER_FACTOR = 0.02f;

    [Header("Inventario")]
    public int inventoryFood;
    public int inventoryWood;
    public int inventoryStone;
    public int inventoryGold;

    public int maxFood = 10;
    public int maxWood = 1;
    public int maxStone = 1;
    public int maxGold = 2;

    Coroutine currentAction;
    float nextDecisionTime;

    public Blackboard Blackboard => blackboard;
    public GameManager GameManager => blackboard.resources; // Acceso directo
    public VillageState Village => GameManager.village;

    private void Awake()
    {
        mover = GetComponent<AgentMover>();
        if (eatAction == null) Debug.LogError("Asignar accion: A_EatFood");
    }
    private void Update()
    {
        // El hambre sube lentamente con el tiempo
        hunger01 = Mathf.Clamp01(hunger01 + Time.deltaTime * HUNGER_FACTOR);

        if (hunger01 == 1f)
        {
            StartCoroutine(Run(eatAction));
            hunger01 = 0; // Reseteamos
        }
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
            Debug.Log("Hay mejor accion, inicio coroutine");
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
