using UnityEngine;
using UnityEngine.UI;

public class BuildingHealthBar : MonoBehaviour
{
    [SerializeField] private BuildingAccidentHandler building;
    [SerializeField] private Image fillImage;
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private Transform cameraTransform;

    private void Start()
    {
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

    private void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        transform.LookAt(transform.position + cameraTransform.forward);
    }

    private void UpdateBar(float health01)
    {
        if (fillImage != null)
            fillImage.fillAmount = health01;

        if (hideWhenFull)
            gameObject.SetActive(health01 < 1f);
    }
}
