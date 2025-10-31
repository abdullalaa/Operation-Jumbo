/*
 * Script to call up respawn menu and then respawn player to attached empty spawn point 
 */

using UnityEngine;
using UnityEngine.UI;

public class FOVPlayerRespawn : MonoBehaviour
{
    public static FOVPlayerRespawn Instance;

    [Header("UI References")]
    public Canvas respawnMenu;        // Assign in Inspector
    public Button respawnButton;      // Assign in Inspector

    private GameObject playerToRespawn;
    private bool isMenuActive = false;
    public bool gameOver = false;
    // Initialized necessary stuff
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
        playerToRespawn = player;
        respawnMenu.gameObject.SetActive(true);
        isMenuActive = true;

    }

    // Called when the button is pressed
    private void OnRespawnButtonClicked()
    {
        if (playerToRespawn != null)
        {
            RespawnPlayer(playerToRespawn);
            gameOver = false;
        }

        respawnMenu.gameObject.SetActive(false);
        isMenuActive = false;
    }

    // Move player to the respawn location
    private void RespawnPlayer(GameObject player)
    {
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
    }

    // Draw sphere around empty to show where spawn point is
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
