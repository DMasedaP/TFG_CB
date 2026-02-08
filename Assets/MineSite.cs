using UnityEngine;

public class MineSite : MonoBehaviour
{
    [SerializeField] private GameObject mineChild; // Arrastrar
    [SerializeField] private bool constructed;

    public bool CanConstruct()
    {
        return !constructed;
    }
    public void Construct()
    {
        if (!CanConstruct()) return;
        constructed = true;
        mineChild.SetActive(true);
        // Aqui se controlara el coste etc...
        // ...
        // ...
        // ...
    }
}
