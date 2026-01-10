using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] resourceTexts; // Los textos de cada recurso
    [SerializeField] Blackboard Blackboard; // Copia del GameManager

    public void UpdateResources()
    {
        // FOOD
        resourceTexts[0].text = Blackboard.village.foodStock.ToString();
        // WOOD
        resourceTexts[1].text = Blackboard.village.woodStock.ToString();
    }
}
