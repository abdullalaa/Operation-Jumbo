/* 
 * Field of View calculation and giving settings through to mesh
 * Sources used (for all FieldOfView scripts): 
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("FOV Size")]
    public float radius;
    [Range(0, 360)]
    public float angle;

    [Header("FOV Detection")]
    public float detectionTime = 0.75f;
    private float timer = 0f;

    [Header("FOV Color")]
    public Color baseColor = Color.white;
    public Color alertColor = Color.red;

    [Header("FOV Layers")]
    public LayerMask obstructionLayer;
    public LayerMask playerLayer;

    [HideInInspector]
    public GameObject playerRef;
    [HideInInspector]
    public bool playerInFOV;


    private void Update()
    {
       FieldOfViewCheck();
    }


    // Check if player is within FOV
    private void FieldOfViewCheck()
    {
        playerInFOV = false;

        // Check within radius, and see what colliders are there
        Collider[] targetsInFOV = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider c in targetsInFOV)
            {
                // Check if the collider is on the Player layer, otherwise skip it
                if ((playerLayer.value & (1 << c.gameObject.layer)) == 0)
                {
                    continue;
                }

                Vector3 directionToTarget = (c.transform.position - transform.position).normalized;
                float distanceToTarget = Vector3.Distance(transform.position, c.transform.position);

                // Angle of FOV
                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    // Check if there is no obstruction
                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionLayer))
                    {
                        playerInFOV = true;
                        playerRef = c.gameObject; // Store reference for editor visualization
                    }
                }

                // Timer starts if player in FOV
                if (playerInFOV)
                {
                    timer += Time.deltaTime;
                    //Debug.Log("Timer" + timer);
                    if (timer >= detectionTime)
                    {
                        FOVPlayerRespawn.Instance.ShowRespawnMenu(playerRef); // Show respawn menu

                    }
                }

                // Reset timer if player is not in FOV
                if (!playerInFOV)
                {
                    timer = 0f;
                }

            }
        }
    }

