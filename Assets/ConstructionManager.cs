using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionManager : MonoBehaviour
{
    #region VARIABLES
    [Header("References")]
    [SerializeField] private Camera mainCamera;    
    [SerializeField] private GameObject buildingBorder; // Asignar en el editor
    [SerializeField] private GameObject buildingMenuPanel; // Panel construccion estructuras

    [Header("Layers")]
    [SerializeField] private LayerMask mineSitesLayer; // Capa de los materiales
    [SerializeField] private LayerMask groundLayer; // Suelo para colocar edificios

    [Header("Grid")]
    [SerializeField] private GridBuildingSystem gridSystem;

    [Header("Ghost")]
    [SerializeField] private Material ghostMaterial;

    [SerializeField] private GameManager gameManager;
    private VillageState villageState;
    private GameObject currentGhost;
    private float currentRotation = 0f;

    public bool BuildMode {  get; private set; }

    private BuildSelectionType currentSelectionType = BuildSelectionType.None;
    private BuildingTypeData selectedBuilding;
    #endregion
    private enum BuildSelectionType
    {
        None,
        Mine,
        Building
    }

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        villageState = FindAnyObjectByType<GameManager>().village; // SI NO FUNCIONA CORRECTAMENTE QUIZAS HAYA QUE HACER VILLAGE ESTATICO
        if (gameManager == null) Debug.LogError("No he podido asignar GameManager");
        if (villageState == null) Debug.LogError("No he podido asignar villageState");
        if(mainCamera == null) mainCamera = Camera.main;
        if (buildingBorder == null) Debug.LogError("Asignar buildingBorder en el editor");
        if (buildingMenuPanel == null) Debug.LogError("Asignar buildingMenuPanel en el editor");
        if (gridSystem == null) Debug.LogError("Asignar el gridSystem del Ground");
        if (ghostMaterial == null) Debug.LogError("Asignar el ghostMaterial en el editor");
    }
    public void ToogleBuildMode()
    {
        BuildMode = !BuildMode;
        buildingBorder.SetActive(BuildMode);
        buildingMenuPanel.SetActive(BuildMode);

        if (!BuildMode) ClearSelection();

        Debug.Log("BUILD MODE: "+ BuildMode);
    }

    public void SelectMineMode()
    {
        BuildMode = true;
        currentSelectionType = BuildSelectionType.Mine;
        selectedBuilding = null;

        DestroyGhost();
    }

    public void SelectBuilding(BuildingTypeData buildingData)
    {
        if(buildingData == null) return;
        currentRotation = 0f;
        BuildMode = true;
        currentSelectionType = BuildSelectionType.Building;
        selectedBuilding = buildingData;

        CreateGhost();
        Debug.Log("Edificio seleccionado: " + buildingData.buildingName);
    }

    public void ClearSelection()
    {
        currentSelectionType = BuildSelectionType.None;
        selectedBuilding = null;
        DestroyGhost();
    }

    private void Update()
    {
        if (!BuildMode) return;

        // Evita clicks sobre UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        switch (currentSelectionType)
        {
            case BuildSelectionType.Mine:
                HandleMineConstruction();
                break;

            case BuildSelectionType.Building:
                HandleBuildingPlacement();
                break;
        }
       
        if (Input.GetKeyDown(KeyCode.R)) RotateBuilding();

        if (Input.GetMouseButtonDown(1))
        {
            ClearSelection();
            Debug.Log("Selección cancelada");
        }
    }

    private void HandleMineConstruction()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        /*if (!CanAffordMine())
        {
            Debug.LogError("No tienes recursos suficientes;");
            return;
        }*/

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, mineSitesLayer))
        {
            MineSite site = hit.collider.GetComponentInParent<MineSite>();
            if (site == null) Debug.LogError("Hago RayCast, pero MinSite es NULL");
            if (site != null && site.CanConstruct() && CanAffordMine(site.type))
            {
                site.Construct();
                PayMineCost(site.type);
                Debug.Log("Mina construida");
            }
        }
    }

    private void HandleBuildingPlacement()
    {
        if (selectedBuilding == null || gridSystem == null) return;

        UpdateGhostPosition();

        if (!Input.GetMouseButtonDown(0)) return;

        if (!GetMouseWorldPosition(out Vector3 worldPos))
            return;

        gridSystem.GetXY(worldPos, out int x, out int y);

        if (!gridSystem.CanPlaceBuilding(x, y, selectedBuilding.width, selectedBuilding.height))
        {
            Debug.Log("No se puede construir ahí");
            return;
        }
        if (!CanAffordSelectedBuilding())
        {
            Debug.LogError("No tienes recursos suficientes;");
            return;
        }

        Vector3 buildPos = gridSystem.GetCellCenterWorld(x, y);
        PayBuildingCost();
        Instantiate(selectedBuilding.prefab, buildPos, Quaternion.Euler(0, currentRotation, 0));

        gridSystem.SetOccupied(x, y, selectedBuilding.width, selectedBuilding.height, true);

        Debug.Log("Edificio colocado: " + selectedBuilding.buildingName);
    }
    private void UpdateGhostPosition()
    {
        if (currentGhost == null) return;
        if (!GetMouseWorldPosition(out Vector3 worldPos)) return;
        if (selectedBuilding == null) return;

        gridSystem.GetXY(worldPos, out int x, out int y);
        Vector3 snappedPos = gridSystem.GetCellCenterWorld(x, y);

        currentGhost.transform.position = snappedPos;

        bool canAfford = CanAffordSelectedBuilding();
        bool canPlace = gridSystem.CanPlaceBuilding(x, y, selectedBuilding.width, selectedBuilding.height);
        Renderer[] renderers = currentGhost.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (canPlace && canAfford)
                    mat.color = new Color(0f, 1f, 0f, 0.35f); // verde
                else
                    mat.color = new Color(1f, 0f, 0f, 0.35f); // rojo
            }
        }
    }

    private bool GetMouseWorldPosition(out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, groundLayer))
        {
            worldPosition = hit.point;
            return true;
        }

        return false;
    }

    private void CreateGhost()
    {
        DestroyGhost();

        if (selectedBuilding == null || selectedBuilding.prefab == null)
            return;

        currentGhost = Instantiate(selectedBuilding.prefab);

        PrepareGhostObject(currentGhost);
        ApplyMaterialToGhost(currentGhost);
    }

    private void DestroyGhost()
    {
        if (currentGhost != null)
            Destroy(currentGhost);
    }

    private void PrepareGhostObject(GameObject ghost)
    {
        // Desactivar todos los scripts
        MonoBehaviour[] behaviours = ghost.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            behaviour.enabled = false;
        }

        // Desactivar colliders
        Collider[] colliders = ghost.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Desactivar física
        Rigidbody[] rigidbodies = ghost.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void ApplyMaterialToGhost(GameObject ghost)
    {
        if (ghostMaterial == null) return;

        Renderer[] renderers = ghost.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            Material[] mats = new Material[rend.materials.Length];

            for (int i = 0; i < mats.Length; i++)
                mats[i] = ghostMaterial;

            rend.materials = mats;
        }
    }

    private void RotateBuilding()
    {
        currentRotation += 45f;

        if(currentRotation >= 360f) currentRotation = 0f;
        if(currentGhost != null) currentGhost.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
    }

    #region Build Cost 
    private bool CanAffordSelectedBuilding()
    {
        if(selectedBuilding == null) return false;
        return villageState.woodStock >= selectedBuilding.woodCost &&
            villageState.stoneStock >= selectedBuilding.stoneCost &&
            villageState.goldStock >= selectedBuilding.goldCost &&
            villageState.foodStock >= selectedBuilding.foodCost;
    }
    private void PayBuildingCost()
    {
        villageState.woodStock -= selectedBuilding.woodCost;
        villageState.foodStock -= selectedBuilding.foodCost;
        villageState.stoneStock -= selectedBuilding.stoneCost;
        villageState.goldStock -= selectedBuilding.goldCost;
        gameManager.uiManager.UpdateResources(); // Actualizamos la UI
    }
    private bool CanAffordMine(MineType type)
    {        
        if (type == MineType.Gold)
        {
            return villageState.woodStock >= 100 &&
                    villageState.foodStock >= 40 &&
                    villageState.stoneStock >= 60;
        }
        else
        {
            return villageState.woodStock >= 80 &&
                    villageState.foodStock >= 25;
        }
    }
    private void PayMineCost(MineType type)
    {
        if(type == MineType.Gold)
        {
            villageState.woodStock -= 110;
            villageState.foodStock -= 40;
            villageState.stoneStock -= 60;
        }
        else
        {
            villageState.woodStock -= 80;
            villageState.foodStock -= 25;
        }
        gameManager.uiManager.UpdateResources(); // Actualizamos la UI
    }
    #endregion
}
