using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Escena")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Paneles")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    public void PlayGame()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("No se ha asignado el nombre de la escena de juego en el MenuManager.");
            return;
        }

        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    public void OpenTutorial()
    {
        OpenPanel(tutorialPanel);
    }

    public void OpenOptions()
    {
        OpenPanel(optionsPanel);
    }

    public void OpenCredits()
    {
        OpenPanel(creditsPanel);
    }
    /// <summary>
    /// Asignar en los botones "VOLVER" de cada panel
    /// </summary>
    public void ShowMainMenu()
    {
        mainPanel.SetActive(true);
        tutorialPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    private void OpenPanel(GameObject panelToOpen)
    {
        if (panelToOpen == null)
        {
            Debug.LogWarning("El panel que intentas abrir no está asignado en el MenuManager.");
            return;
        }

        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        panelToOpen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

