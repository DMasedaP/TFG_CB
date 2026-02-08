using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask mineSitesLayer; // Capa de los materiales
    [SerializeField] private GameObject buildingBorder; // Asignar en el editor
    public bool BuildMode {  get; private set; }

    private void Awake()
    {
        if(mainCamera == null) mainCamera = Camera.main;
    }
    public void ToogleBuildMode()
    {
        BuildMode = !BuildMode;
        Debug.LogError("TOGGLE BUILD");
        buildingBorder.SetActive(BuildMode);
        // Añadir algun visual o algo
    }
    private void Update()
    {
        if (!BuildMode) return;

        // Evita clicks atravesando UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 500f, mineSitesLayer))
            {
                MineSite site = hit.collider.GetComponentInParent<MineSite>();
                if (site != null && site.CanConstruct())
                {
                    site.Construct();

                    // Opcional: salir de modo construcción tras construir una
                    // BuildMode = false;
                }
            }
        }
    }
}
