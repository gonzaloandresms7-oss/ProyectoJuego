using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject pausePanel;
    public New_CharacterController player;
    public StartScreen startScreen;
    public IntroVideo intro;

    [Header("Opciones")]
    public KeyCode pauseKey = KeyCode.Escape;
    public bool pauseAudio = true;

    public static bool IsPaused { get; private set; }

    private CursorLockMode prevLock;
    private bool prevCursorVisible;

    void Start()
    {
        IsPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        if (!Input.GetKeyDown(pauseKey)) return;

        if (IsPaused) { Resume(); return; }

        if (GameOverScreen.IsShown) return;

        if (startScreen != null && !startScreen.GameStarted) return;
        if (intro != null && intro.BlocksStart) return;

        Pause();
    }


    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;

        Time.timeScale = 0f;
        if (pauseAudio) AudioListener.pause = true;
        if (player != null) player.enabled = false;

        prevLock = Cursor.lockState;
        prevCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            pausePanel.transform.SetAsLastSibling();   
        }
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;

        if (pausePanel != null) pausePanel.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (player != null) player.enabled = true;

        Cursor.lockState = prevLock;
        Cursor.visible = prevCursorVisible;
    }

    public void Restart()
    {
        LeavePause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        LeavePause();
        StartScreen.ShowOnNextLoad();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LeavePause()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}