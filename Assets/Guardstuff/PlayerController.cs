using UnityEngine;
using UnityEngine.UIElements;
//attach to the player, should be more fletched out
public class PlayerController : MonoBehaviour
{
    public float speed = 2;
    private bool nextLevel;
    private GameObject Level2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Level2 = GameObject.Find("SpawnLvl2"); //initialize gameobject 
    }

    // Update is called once per frame
    void Update()
    {
        //every frame the player character moves forward
        
        transform.Translate(Vector3.forward * speed); //makes player move automatically for testing
        
        Transform levelPos = Level2.GetComponent<Transform>(); //get transform of next level spawnpoint (is an empty called spawnLvl2
        nextLevel = GameObject.Find("levelendTrigger").GetComponent<levelTransitioner>().nextLevelTransition; //Bool that says if player has completed level

        if (nextLevel) //if true trigger teleport to next level
        {
            gameObject.transform.position = levelPos.position;
            
        }
    }
}
