using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource currentSong;
    public AudioSource slowStep;
    public AudioSource fastStep;
    public AudioSource mediumStep;

    private AudioSource previousSong;

    public void ChangeSound(AudioSource newSound)
    {
        if (currentSong == newSound) return;
        if (currentSong != null)
        {
            currentSong.mute = true;
        }

        currentSong = newSound;
        currentSong.mute = false;
    }

}
