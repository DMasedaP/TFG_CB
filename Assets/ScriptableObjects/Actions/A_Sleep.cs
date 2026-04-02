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
        return DayNightCycle.Instance != null && DayNightCycle.Instance.IsNight;
    }
    public override IEnumerator Execute(UtilityAgent agent)
    {
        if (DayNightCycle.Instance == null)
            yield break;

        agent.isSleeping = false;
        agent.sleptOutsideLastNight = false;

        House assignedHouse = null;
        Vector3 targetPos = agent.transform.position;

        if (PopulationManager.Instance != null && PopulationManager.Instance.TryAssignHouse(agent, out assignedHouse))
        {
            agent.assignedHouse = assignedHouse;
            targetPos = assignedHouse.SleepPosition;
        }
        else
        {
            agent.assignedHouse = null;
            agent.sleptOutsideLastNight = true;
            targetPos = agent.transform.position;
        }

        if (agent.mover != null)
            agent.mover.GoTo(targetPos, 1.2f);

        while (DayNightCycle.Instance.IsNight)
        {
            /*float dist = Vector3.Distance(agent.transform.position, targetPos);

            if (dist <= arriveDistance)
            {
                agent.isSleeping = true;
                if (agent.mover != null)
                    agent.mover.Stop();
            }*/
            yield return new WaitForSeconds(sleepCheckInterval);
        }

        agent.isSleeping = false;
    }
}
