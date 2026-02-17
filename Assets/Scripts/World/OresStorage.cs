using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OresStorage : MonoBehaviour
{
    public GameManager gm;
    public Transform dropPoint;

    public void DepositStone(int amount)
    {
        gm.TryAddStone(amount);
    }
    public void DepositGold(int amount)
    {
        gm.TryAddGold(amount);
    }
}
