using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OresStorage : MonoBehaviour
{
    public Transform dropPoint;
    public GameManager gm;
    [SerializeField] GameObject[] goldVisuals; // Length = 16
    [SerializeField] GameObject[] stoneVisuals; // Length = 4

    private void Start()
    {
        if(gm == null)
        {
            Debug.LogError("Asignar en el inspector un GameManager");
            gm = FindObjectOfType<GameManager>();
        }
    }
    public void UpdateGameObjectVisuals()
    {
        // Maximo de oro es 100 y tenemos 16 GameObjects

        // Maximo de piedra es 16 y tenemos 4(4unidades) GameObjects
        // hay q conseguir dividir el total de tal forma que de resultado en grupos de 4
        var index = (int)(gm.village.stoneStock / 4); // si tenemos 6 da 1, si tenemos 11 da 2
        for (int i = 0; i < stoneVisuals.Length; i++)
        {
            if(i <= index) stoneVisuals[i].SetActive(true);
            else stoneVisuals[i].SetActive(false);
        }
    }
}
