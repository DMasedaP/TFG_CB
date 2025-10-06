using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityParameters : EditorBehaviourRunner
{
    /*
     * COMIDA
     * MADERA
     * PIEDRA
     * ORO
     * ESPACIO VIVIENDAS
     */

    // Ejemplo
    // Con esta config deberian aguantar 2 dias
    private int citizenNUM = 2;
    private int foodConsum = 2; // per day
    public int foodAmount = 4;

    public float Hunger() => foodAmount / citizenNUM;

    public void GatherFood()
    {
        Debug.Log("Gather...");
        foodAmount += 4;
    }
    public void Sleep()
    {
        Debug.Log("ZZZ...");
        foodAmount -= 1;
    }
}
