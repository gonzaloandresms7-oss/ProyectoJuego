using System.Collections;
using TMPro;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject startPanel;
    public CanvasGroup panelGroup;
    public New_CharacterController player;
    public IntroVideo intro;

    [Header("Inicio")]
    public KeyCode startKey = KeyCode.Space;
    public float fadeOutTime = 0.35f;

    [Header("Texto parpadeante (opcional)")]
    public TextMeshProUGUI promptText;
    public CanvasGroup promptGroup;
    public float pulseSpeed = 3f;

    [Header("Música de fondo")]
    public AudioSource musicSource;   // el mismo AudioSource de la música ambiental

    private bool started;

    private static bool alreadyStarted;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { alreadyStarted = false; }

    public bool GameStarted => started;

    public static void ShowOnNextLoad() { alreadyStarted = false; }

    void Awake()
    {
        if (alreadyStarted)
        {
            started = true;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            if (startPanel != null) startPanel.SetActive(false);
            if (panelGroup != null) panelGroup.alpha = 0f;
            if (player != null) player.enabled = true;
            EnsureMusicPlaying();
            return;
        }

        Time.timeScale = 0f;

        AudioListener.pause = true;

        if (startPanel != null)
        {
            startPanel.SetActive(true);
            startPanel.transform.SetAsLastSibling();
        }
        if (panelGroup != null) panelGroup.alpha = 1f;

        if (player != null) player.enabled = false;
    }

    void Update()
    {
        if (started) return;

        float a = Mathf.Lerp(0.45f, 1f, (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f);
        if (promptGroup != null) promptGroup.alpha = a;
        if (promptText != null)
        {
            Color c = promptText.color;
            c.a = a;
            promptText.color = c;
        }

        if (intro != null && intro.BlocksStart) return;

        if (Input.GetKeyDown(startKey))
            StartCoroutine(BeginGame());
    }

    IEnumerator BeginGame()
    {
        started = true;

        if (panelGroup != null && fadeOutTime > 0f)
        {
            float t = 0f;
            while (t < fadeOutTime)
            {
                t += Time.unscaledDeltaTime;
                panelGroup.alpha = 1f - Mathf.Clamp01(t / fadeOutTime);
                yield return null;
            }
            panelGroup.alpha = 0f;
        }

        Time.timeScale = 1f;
        AudioListener.pause = false;
        EnsureMusicPlaying();

        yield return null;

        if (player != null) player.enabled = true;

        alreadyStarted = true;

        if (startPanel != null) startPanel.SetActive(false);

        Debug.Log("StartScreen: juego iniciado");
    }
    void EnsureMusicPlaying()
{
    var music = MusicManager.Instance;
    if (music == null)
    {
        Debug.LogWarning("StartScreen: no hay MusicManager en la escena");
        return;
    }
    music.PlayMusic();
}
}