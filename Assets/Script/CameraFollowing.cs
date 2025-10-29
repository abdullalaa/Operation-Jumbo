using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public Transform player;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 4f, -10f);
    public float followSpeed = 5f;
    public float rotateSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed* Time.deltaTime);

        Quaternion targetRot = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed* Time.deltaTime);  
    }
}
