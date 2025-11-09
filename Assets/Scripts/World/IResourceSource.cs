using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceSource
{
    string ResourceId { get; }
    int Harvest(GameObject worker);
}
