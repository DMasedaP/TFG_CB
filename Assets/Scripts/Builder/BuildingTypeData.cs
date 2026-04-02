using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TFG/Building Type")]
public class BuildingTypeData : ScriptableObject
{
    public string buildingName;
    public Sprite icon;
    public GameObject prefab;

    [Header("Grid Size")]
    public int width = 1;
    public int height = 1;

    [Header("Costes")]
    public int woodCost;
    public int stoneCost;
    public int goldCost;

    [Header("Housing")]
    public bool isHouse;
}
