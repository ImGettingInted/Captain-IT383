using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public AudioSource uiAudioSource;
    public AudioClip clickSound;

    public void StartGame()
    {
        uiAudioSource.PlayOneShot(clickSound);
        SceneManager.LoadScene("BasicScene");
    }
}