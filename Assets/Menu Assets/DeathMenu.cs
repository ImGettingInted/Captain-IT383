using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathUI;

    [Header("SFX")]
    public AudioSource uiAudioSource;
    public AudioClip clickSound;

    void Start()
    {
        Time.timeScale = 1f;
        deathUI.SetActive(true);
    }

    void PlayClick()
    {
        if (uiAudioSource != null && clickSound != null)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }

    public void Restart()
    {
        PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene("BasicScene");
    }

    public void MainMenu()
    {
        PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ShowDeath()
    {
        deathUI.SetActive(true);
        Time.timeScale = 0f;
    }
}