using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public GameManager gm;

    public void RegisterCitizen()   => gm.village.citizens++;
    public void UnregisterCitizen() => gm.village.citizens = Mathf.Max(0, gm.village.citizens - 1);
    public void RegisterArmy()      => gm.village.army++;
    public void UnregisterArmy()    => gm.village.army = Mathf.Max(0, gm.village.army - 1);
}
