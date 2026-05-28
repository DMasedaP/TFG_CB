using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/US/Action/Sleep")]
public class A_Sleep : UtilityAction
{
    [Header("Sleep")]
    public float sleepCheckInterval = 0.2f;

    public override bool CanRun(UtilityAgent agent)
    {
        return DayNightCycle.Instance != null &&
           (DayNightCycle.Instance.IsNight || agent.isSleeping);
    }
    /*public override IEnumerator Execute(UtilityAgent agent)
    {
        if (DayNightCycle.Instance == null)
            yield break;

        agent.isSleeping = false;
        agent.sleptOutsideLastNight = false;

        House assignedHouse = null;
        Vector3 targetPos = agent.transform.position;

        if (PopulationManager.Instance != null && PopulationManager.Instance.TryAssignHouse(agent, out assignedHouse))
        {
            Debug.LogError("Entro IF POPULATIONMANAGER");
            agent.assignedHouse = assignedHouse;
            targetPos = assignedHouse.SleepPosition;
            Debug.LogError($"targetPos-> {targetPos}");
        }
        else
        {
            agent.assignedHouse = null;
            agent.sleptOutsideLastNight = true;
            targetPos = agent.transform.position;
        }
        yield return agent.mover.GoTo(targetPos, 1.2f);

        if (agent.assignedHouse == null)
        {
            agent.mover.LayDown(); // Nos tumbamos, ESTO HACE STOP()
            while (DayNightCycle.Instance.IsNight)
            {
                float dist = Vector3.Distance(agent.transform.position, targetPos);

                if (dist <= 0.5f)
                {
                    agent.isSleeping = true;
                }
                yield return new WaitForSeconds(sleepCheckInterval);
            }
            agent.mover.StandUp(); // Nos levantamos
            agent.isSleeping = false;
        }
    }*/
    public override IEnumerator Execute(UtilityAgent agent)
    {
        if (DayNightCycle.Instance == null)
            yield break;

        agent.isSleeping = false;
        agent.sleptOutsideLastNight = false;

        House assignedHouse = null;
        Vector3 targetPos = agent.transform.position;
        bool sleepsInHouse = false;

        if (PopulationManager.Instance != null && PopulationManager.Instance.TryAssignHouse(agent, out assignedHouse))
        {
            agent.assignedHouse = assignedHouse;
            targetPos = assignedHouse.SleepPosition;
            sleepsInHouse = true;

            Debug.Log($"El civil va a dormir en casa. SleepPoint: {targetPos}");
        }
        else
        {
            agent.assignedHouse = null;
            agent.sleptOutsideLastNight = true;
            targetPos = agent.transform.position;
            sleepsInHouse = false;

            Debug.Log("No hay casa disponible. El civil dormirá fuera.");
        }

        // Si tiene casa, primero va al SleepPoint.
        if (sleepsInHouse)
        {
            yield return agent.mover.GoTo(targetPos, 1.2f);

            // Si mientras iba a la casa se hizo de día, no debe ocultarse.
            if (!DayNightCycle.Instance.IsNight)
                yield break;

            agent.isSleeping = true;

            bool hasWokenUp = false;

            Action wakeUp = null;
            wakeUp = () =>
            {
                if (hasWokenUp)
                    return;

                hasWokenUp = true;

                if (DayNightCycle.Instance != null)
                    DayNightCycle.Instance.OnDayStarted -= wakeUp;

                Debug.Log("Ha amanecido. Muestro al civil que dormía en casa.");

                agent.mover.ShowVisual();

                // Lo colocamos en el SleepPoint de la casa.
                agent.transform.position = targetPos;

                agent.mover.StandUp();

                agent.isSleeping = false;
            };

            DayNightCycle.Instance.OnDayStarted += wakeUp;

            // Simula que ha entrado en la casa.
            agent.mover.HideVisual();

            while (DayNightCycle.Instance != null && DayNightCycle.Instance.IsNight)
            {
                yield return new WaitForSeconds(sleepCheckInterval);
            }

            // Por si la corrutina NO se ha cortado, despertamos también aquí.
            wakeUp();
        }
        else
        {
            // Si no tiene casa, duerme fuera como antes.
            agent.mover.LayDown();
            agent.isSleeping = true;

            while (DayNightCycle.Instance.IsNight)
            {
                yield return new WaitForSeconds(sleepCheckInterval);
            }

            agent.mover.StandUp();
            agent.isSleeping = false;
        }
    }
}
