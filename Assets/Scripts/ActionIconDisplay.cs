using UnityEngine;
using UnityEngine.UI;

public class ActionIconDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;

    [Header("Billboard")]
    [SerializeField] private bool lookAtCamera = true;

    private Camera mainCamera;

    private void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponentInChildren<Image>(true);

        mainCamera = Camera.main;

        HideIcon();
    }

    private void LateUpdate()
    {
        if (!lookAtCamera || mainCamera == null)
            return;

        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    }

    public void SetIcon(Sprite sprite)
    {
        if (iconImage == null)
            return;

        if (sprite == null)
        {
            HideIcon();
            return;
        }

        iconImage.sprite = sprite;
        iconImage.enabled = true;
        gameObject.SetActive(true);
    }

    public void HideIcon()
    {
        if (iconImage != null)
            iconImage.enabled = false;
    }
}
