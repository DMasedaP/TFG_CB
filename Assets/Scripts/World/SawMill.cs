using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMill : MonoBehaviour
{
    public GameManager gm;
    public Transform dropPoint;

    public void DepositWood(int amount)
    {
        gm.TryAddWood(amount);
    }
}
