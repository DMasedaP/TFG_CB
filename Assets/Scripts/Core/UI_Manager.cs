using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] resourceTexts; // Los textos de cada recurso
    [SerializeField] Blackboard Blackboard; // Copia del GameManager

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(.5f);
        UpdateResources();
    }
    public void UpdateResources()
    {
        // FOOD
        resourceTexts[0].text = Blackboard.village.foodStock.ToString();
        // WOOD
        resourceTexts[1].text = Blackboard.village.woodStock.ToString();
        // GOLD
        resourceTexts[2].text = Blackboard.village.goldStock.ToString();
        // STONE
        resourceTexts[3].text = Blackboard.village.stoneStock.ToString();
        // CIVILS
        resourceTexts[4].text = Blackboard.village.citizens.ToString();
    }
}
