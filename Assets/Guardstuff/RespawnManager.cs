/*
 * Script to call up respawn menu and then respawn player to attached empty spawn point 
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FOVPlayerRespawn : MonoBehaviour
{
    public static FOVPlayerRespawn Instance;

    [Header("UI References")]
    public Canvas respawnMenu;        // Assign in Inspector
    public Button respawnButton;      // Assign in Inspector


    private bool isMenuActive = false;
    public bool gameOver = false;

    private void Awake()
    {
        Instance = this;

        if (respawnMenu != null)
            respawnMenu.gameObject.SetActive(false); // Hide at start

        if (respawnButton != null)
            respawnButton.onClick.AddListener(OnRespawnButtonClicked);
    }

    // Called when player is spotted
    public void ShowRespawnMenu(GameObject player)
    {

        if (isMenuActive) return; // Prevent multiple calls
        gameOver = true;
        
        respawnMenu.gameObject.SetActive(true);
        isMenuActive = true;
    }

    // Called when the button is pressed
    private void OnRespawnButtonClicked()
    {

        ReloadCurrentScene();

        respawnMenu.gameObject.SetActive(false);
        isMenuActive = false;
    }
    public void ReloadCurrentScene()
    {
 
        // Get the active scene
        Scene currentScene = SceneManager.GetActiveScene();


        // Reload the active scene
        SceneManager.LoadScene(currentScene.name);
    }
    
    // Draw sphere around empty to show where spawn point is
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
