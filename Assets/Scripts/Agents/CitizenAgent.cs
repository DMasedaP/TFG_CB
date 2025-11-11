using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenAgent : MonoBehaviour
{
    private PopulationManager popManager;

    private void Start()
    {
        popManager = FindObjectOfType<PopulationManager>();
        popManager?.RegisterCitizen();
    }

    private void OnDestroy()
    {
        if(popManager != null) popManager.UnregisterCitizen();
    }
}
