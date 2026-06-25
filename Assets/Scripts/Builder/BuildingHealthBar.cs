using UnityEngine;
using UnityEngine.UI;

public class BuildingHealthBar : MonoBehaviour
{
    [SerializeField] private BuildingAccidentHandler building;
    [SerializeField] private Image fillImage;
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private Transform cameraTransform;

    // Sorting
    private string sortingLayerName = "UI";
    private int sortingOrder = 100;
    private Canvas canvas;

    private void Start()
    {
        // Sorting
        canvas = GetComponent<Canvas>();
        if (canvas == null) Debug.LogError("No he detectado HealthBarCanvas");
        else
        {
            canvas.overrideSorting = true;
            canvas.sortingLayerName = sortingLayerName;
            canvas.sortingOrder= sortingOrder;
        }

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (building != null)
            building.OnHealthChanged += UpdateBar;

        UpdateBar(building != null ? building.Health01 : 1f);
    }

    private void OnDestroy()
    {
        if (building != null)
            building.OnHealthChanged -= UpdateBar;
    }

    private void Update()
    {
        UpdateBar(building.Health01);
    }
    private void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }

    private void UpdateBar(float health01)
    {
        if (fillImage != null)
            fillImage.fillAmount = health01;

        if (hideWhenFull)
            gameObject.SetActive(health01 < 1f);
    }
}
