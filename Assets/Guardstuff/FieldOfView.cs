/* 
 * Field of View calculation and giving settings through to mesh
 * Sources used (for all FieldOfView scripts): 
 * https://medium.com/codex/creating-a-basic-field-of-vision-system-in-unity-c-718b58951cf6
 * https://www.youtube.com/watch?v=j1-OyLo77ss
 * https://www.youtube.com/watch?v=73Dc5JTCmKI&list=PLoLbLcSqUXwGPCsT3TFZ25i-Dj46NM1Hm&index=13
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

            // Timer starts if player in FOV, and resets when not
            if (playerInFOV)
            {
                timer += Time.deltaTime;
                //Debug.Log("Timer" + timer);
                if (timer >= detectionTime)
                {
                    FOVPlayerRespawn.Instance.ShowRespawnMenu(playerRef); // Show respawn menu
                    
                }
            }
            if (!playerInFOV)
            {
                timer = 0f;
            }

        }
    }
}

