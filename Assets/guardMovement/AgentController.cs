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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //setting begin- and endpoints
        point1 = enemy.transform.position;
        point2 = endPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        //if enemy is at point 1 set destination to point 2 after waiting for waitDuration (seconds)
        if (enemy.transform.position.x == point1.x && enemy.transform.position.z == point1.z)
        {
            waitTimer += Time.deltaTime; 
            if (waitTimer > waitDuration)
            {
                waitTimer = 0;
                enemy.destination = point2;
            }
            Debug.Log("looking for point2");
        }
        //if enemy is at point 2 set destination to point 1 after waiting for waitDuration (seconds)
        else if (enemy.transform.position.x == point2.x && enemy.transform.position.z == point2.z)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer > waitDuration)
            {
                waitTimer = 0;
                enemy.destination = point1;
            }
            Debug.Log("looking for point1");
        }
        

    }
}
