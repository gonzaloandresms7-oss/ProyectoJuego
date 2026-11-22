using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject gameOverPanel;
    public CanvasGroup panelGroup;
    public New_CharacterController player;

    [Header("Opciones")]
    public float fadeInTime = 0.4f;
    public bool pauseAudio = true;
    public AudioClip gameOverClip;
    [Range(0f, 1f)] public float clipVolume = 0.9f;

    public static bool IsShown { get; private set; }

    private bool showing;

    void Start()
    {
        IsShown = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void Show()
    {
        if (showing) return;
        showing = true;
        IsShown = true;

        Time.timeScale = 0f;
        if (pauseAudio) AudioListener.pause = true;
        if (player != null) player.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling();   
        }
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);

        if (gameOverClip != null)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.ignoreListenerPause = true;
            src.spatialBlend = 0f;
            src.PlayOneShot(gameOverClip, clipVolume);
        }

        if (panelGroup != null && fadeInTime > 0f)
            StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        panelGroup.alpha = 0f;
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime;   
            panelGroup.alpha = Mathf.Clamp01(t / fadeInTime);
            yield return null;
        }
        panelGroup.alpha = 1f;
    }

    public void Restart()
    {
        Leave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Leave();
        StartScreen.ShowOnNextLoad();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Leave()
    {
        IsShown = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}