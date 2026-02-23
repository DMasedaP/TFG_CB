using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTickSystem : MonoBehaviour
{
    public GameManager gm;
    public float tickSeconds = 60f;
    
    private void Awake()
    {
        if (gm == null) Debug.LogError("¡No se ha asigando GameManager! > StoneTickSystem.cs");
    }

    private IEnumerator Start()
    {
        var village = gm.village;
        var wait = new WaitForSeconds(tickSeconds);
        while (true)
        {
            yield return wait;
            gm.TryConsumeStone(1);
        }
    }
}
