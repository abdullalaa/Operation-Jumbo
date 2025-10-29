using UnityEngine;

public class SpawnManager : MonoBehaviour

{
    private float waitDuration;
    private float waitTime;
    private bool destroyAgents;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destroyAgents = GameObject.Find("levelendTrigger").GetComponent<levelTransitioner>().destroyAgents;
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyAgents)
        {
            GameObject[] gos = GameObject.FindGameObjectsWithTag("Agent");
            foreach (GameObject go in gos)
                Destroy(go);
        }
    }
    void Spawn()
    {

    }
}
