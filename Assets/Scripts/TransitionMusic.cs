using Unity.VisualScripting;
using UnityEngine;
public class MusicChanger : MonoBehaviour
{
    public GameObject player;      // Reference to player
    private MusicManager mm;


    private void Start()
    {
        mm = FindObjectOfType<MusicManager>();
        
    }

    private void Update()
    {
        if (player == null || mm == null) return;

        // Change music based on tag
        if (player.GetComponent<PlayerControl>().forwardInput != 0 || player.GetComponent<PlayerControl>().sideInput != 0)
        {
            //mm.currentSong.mute = false;
            if (player.CompareTag("Small"))
            {
                mm.ChangeSound(mm.slowStep);
                mm.currentSong.mute = false;
            }
            else if (player.CompareTag("Medium"))
            {
                mm.ChangeSound(mm.mediumStep);
                mm.currentSong.mute = false;
            }
            else if (player.CompareTag("Large"))
            {
                mm.ChangeSound(mm.fastStep);
                mm.currentSong.mute = false;
            }
            else if (player.CompareTag("Float"))
            {
                mm.ChangeSound(mm.balloon);
                mm.currentSong.mute = false;
                
            }
        }
        else if ((player.GetComponent<PlayerControl>().forwardInput == 0 && player.GetComponent<PlayerControl>().sideInput == 0))
        {
            mm.currentSong.mute = true;
        }
        
    }

}

