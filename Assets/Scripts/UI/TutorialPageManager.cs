using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPageManager : MonoBehaviour
{
    [Header("Páginas del tutorial")]
    [SerializeField] private Sprite[] tutorialPages;

    [Header("Referencias UI")]
    [SerializeField] private Image pageImage;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    [Header("Configuración")]
    [SerializeField] private bool resetToFirstPageOnOpen = true;
    [SerializeField] private bool loopPages = false;

    private int currentPageIndex = 0;

    private void OnEnable()
    {
        if (resetToFirstPageOnOpen)
        {
            currentPageIndex = 0;
        }

        UpdatePage();
    }

    public void NextPage()
    {
        if (tutorialPages == null || tutorialPages.Length == 0)
            return;

        if (currentPageIndex < tutorialPages.Length - 1)
        {
            currentPageIndex++;
        }
        else if (loopPages)
        {
            currentPageIndex = 0;
        }

        UpdatePage();
    }

    public void PreviousPage()
    {
        if (tutorialPages == null || tutorialPages.Length == 0)
            return;

        if (currentPageIndex > 0)
        {
            currentPageIndex--;
        }
        else if (loopPages)
        {
            currentPageIndex = tutorialPages.Length - 1;
        }

        UpdatePage();
    }

    private void UpdatePage()
    {
        if (tutorialPages == null || tutorialPages.Length == 0)
        {
            Debug.LogWarning("No hay páginas asignadas en el TutorialPageManager.");
            return;
        }

        currentPageIndex = Mathf.Clamp(currentPageIndex, 0, tutorialPages.Length - 1);

        if (pageImage != null)
        {
            pageImage.sprite = tutorialPages[currentPageIndex];
            pageImage.preserveAspect = true;
        }

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        if (previousButton != null)
        {
            previousButton.interactable = loopPages || currentPageIndex > 0;
        }

        if (nextButton != null)
        {
            nextButton.interactable = loopPages || currentPageIndex < tutorialPages.Length - 1;
        }
    }
}