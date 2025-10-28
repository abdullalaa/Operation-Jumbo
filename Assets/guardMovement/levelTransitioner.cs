using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
//attach to box collider with isTrigger enabled
public class levelTransitioner : MonoBehaviour
{
    private Vector3 playerPos;
    public GameObject nextLevel;
    public bool nextLevelTransition = false;
    public bool destroyAgents = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("endlevelTrigger entered");
            
            destroyAgents = true;
            nextLevelTransition = true;
           
            
        }

    }
    private void OnTriggerExit(Collider other)
    {
        nextLevelTransition = false;
        destroyAgents = false;
    }
}
