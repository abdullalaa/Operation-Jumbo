using UnityEngine;
using System.Collections;
//attach to an empty that you want to be the respawn location of the player
public class FOVPlayerRespawn : MonoBehaviour
{
    public static FOVPlayerRespawn Instance; // Singleton for easy access, no longer need to reference in inspector and only 1 respawn point

    private bool canRespawn = true;

    private void Awake()
    {
        Instance = this;
    }
    
    // Public method to respawn player
    public void RespawnPlayer(GameObject player)
    {
        if (canRespawn && player != null)
        {
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        }
    }

    // See empty game object
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f); // Draw sphere at respawn point
    }
}
