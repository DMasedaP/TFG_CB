using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granary : MonoBehaviour
{
    public GameManager gm;
    public Transform dropPoint;

    public void DepositFood(int amount)
    {
        gm.TryAddFood(amount);
    }
}
