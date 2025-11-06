using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;     // The main menu panel with Start/Controls/Quit buttons
    public GameObject controlsPanel;     // The panel showing the controls info

    [Header("Buttons")]
    public Button startButton;
    public Button controlsButton;
    public Button returnButton;

    void Start()
    {
        // Ensure correct visibility
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);

        // Hook up button listeners
        if (startButton != null)
            startButton.onClick.AddListener(OnStartGame);

        if (controlsButton != null)
            controlsButton.onClick.AddListener(OnShowControls);

        if (returnButton != null)
            returnButton.onClick.AddListener(OnReturnToMenu);
    }

    // Called when "Start Game" button is pressed
    public void OnStartGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene found in Build Settings!");
        }
    }

    // Called when "Controls" button is pressed
    public void OnShowControls()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    // Called when "Return" or "Back" button is pressed
    public void OnReturnToMenu()
    {
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
