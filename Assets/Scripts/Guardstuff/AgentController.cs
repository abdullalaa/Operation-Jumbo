using System;
using UnityEngine;
using UnityEngine.AI;
//attach to agent/guards
public class AgentController : MonoBehaviour
{
    //Setting different gameObjects
    /*
      enemy Agent settings used (radius 0.75, Quality = none)
      NavmeshSurface is set by making the walls-layer a not-walkable surface. You can use invisible walls with a boxcollider
      and setting the geometry to use Physics objects to make certain areas not accesible to the enemy

    in enemy move the Agent
    in EndPoint, make an empty, place at position and drag it into End point in inspector
    
     */
    public NavMeshAgent enemy;
    public GameObject endPoint; //use a cylinder or empty game object to easily set the end position

    //initializing variables/vectors
    public Vector3 point1;
    public Vector3 point2;
    private float waitTimer;
    public float waitDuration = 1;
    public Animator MouseMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //setting begin- and endpoints
        point1 = enemy.transform.position;
        point2 = endPoint.transform.position;
        //MouseMovement = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameOver)
        {
            MouseMovement.SetBool("guardWalk", false);
            return;
        }
        MouseMovement.SetBool("guardWalk", true);
        enemy.updateRotation = true;
        if (Vector3.Distance(enemy.transform.position, point1) < 0.1f)
        {
            MouseMovement.SetBool("guardWalk", false);

            enemy.updateRotation = false;
            waitTimer += Time.deltaTime;

            if (waitTimer > waitDuration)
            {
                // Rotate 180 degrees
                enemy.transform.rotation *= Quaternion.Euler(0, 180, 0);

                waitTimer = 0;

                // Move to point2
                enemy.SetDestination(point2);
                
            }
        }
        else if (Vector3.Distance(enemy.transform.position, point2) < 0.1f)
        {
            MouseMovement.SetBool("guardWalk", false);
            enemy.updateRotation = false;
            waitTimer += Time.deltaTime;
            if (waitTimer > waitDuration)
            {
                
                enemy.transform.rotation *= Quaternion.Euler(0, 180, 0);
                waitTimer = 0;
                enemy.destination = point1;
                enemy.updateRotation = true;
            }
            //Debug.Log("looking for point1");
        }
        

    }
}
