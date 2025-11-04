using UnityEngine;

public class musicManager : MonoBehaviour
{
    private AudioSource currentSong = null;
    
    
    public AudioSource CurrentSong
    {
        get { return currentSong; }
        set { currentSong = value; }
    }



}

