/* 
 * Field of View calculation and giving settings through to mesh
 * Sources used (for all FieldOfView scripts): 
 * https://medium.com/codex/creating-a-basic-field-of-vision-system-in-unity-c-718b58951cf6
 * https://www.youtube.com/watch?v=j1-OyLo77ss
 * https://www.youtube.com/watch?v=73Dc5JTCmKI&list=PLoLbLcSqUXwGPCsT3TFZ25i-Dj46NM1Hm&index=13
 * 
 * FOV obstruction layer = wall
    player layer = Player
 */

//attach to guard

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("FOV Size")]
    public float radius;
    [Range(0, 360)]
    public float angle;

    [Header("FOV Color")]
    public float colorTransitionSpeed = 2f;
    public Color baseColor = Color.white;
    public Color alertColor = Color.red;

    [Header("FOV Layers")]
    public LayerMask obstructionLayer;
    public LayerMask playerLayer;

    [HideInInspector]
    public GameObject playerRef;
    [HideInInspector]
    public bool playerInFOV;

    private void Start()
    {
        //playerRef = GameObject.FindGameObjectWithTag("Player"); // Automatically assign player as playerRef
        //StartCoroutine(FOVRoutine());
    }

    private void Update()
    {
        FieldOfViewCheck();
    }

    private IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // This can also be done in update, but if having multiple things in update, this will reduce lag.
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        playerInFOV = false;
        playerRef = null;

        Collider[] targetsInFOV = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider c in targetsInFOV)
        {
            // Check if the collider is on the Player layer mask
            if ((playerLayer.value & (1 << c.gameObject.layer)) == 0)
                continue;

            // Use this collider's transform, not playerRef (which is still null)
            Vector3 directionToTarget = (c.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, c.transform.position);

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                // Check obstruction
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
                {
                    playerInFOV = true;
                    playerRef = c.gameObject; // Store reference for editor visualization
                    FOVPlayerRespawn.Instance.RespawnPlayer(playerRef); // Respawn player
                    Debug.Log("Player seen");
                    break; // Stop after first visible player
                }
            }
        }

        //if (!playerInFOV)
        //    Debug.Log("Player not seen");
    }

}
