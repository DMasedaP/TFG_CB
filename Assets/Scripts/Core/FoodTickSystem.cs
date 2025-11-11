using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTickSystem : MonoBehaviour
{
    public GameManager gm;
    public float tickSeconds = 5f;

    private IEnumerator Start()
    {
        var village = gm.village;
        var wait = new WaitForSeconds(tickSeconds);

        while (true)
        { // Cada 5 segs
            int consume = Mathf.CeilToInt(village.FoodPerTick);
            gm.TryConsumeFood(consume);
            Debug.LogError($"FoodTickSystem: Consumo {consume} de comida.");
            yield return wait;
        }
    }
}
