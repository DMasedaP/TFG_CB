using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkSpot : MonoBehaviour
{
    [NonSerialized] public GameObject reservedBy;
    void Awake()
    {
        reservedBy = null;
    }
}
