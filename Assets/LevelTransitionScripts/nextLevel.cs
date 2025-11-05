using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class nextLevel : MonoBehaviour
{
    public Animator transition;
    public float transitionTime;
    private bool next = false;
    private GameObject Player;
    private void Start()
    {
        Player = GameObject.Find("playerMedium");
    }
    public void Update()
    {
        next = Player.GetComponent<InteractionWEndPoint>().isConnected;
        if (next)
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));

    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);

    }
}
