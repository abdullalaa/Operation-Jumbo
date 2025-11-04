using UnityEngine;

public class PlantWiggleController : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            anim.SetBool("PlantWiggle", true);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            anim.SetBool("PlantWiggle", false);
        }

    }
}
