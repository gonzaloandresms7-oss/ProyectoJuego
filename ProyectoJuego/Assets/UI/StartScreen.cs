using System.Collections;
using TMPro;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject startPanel;
    public CanvasGroup panelGroup;
    public New_CharacterController player;

    [Header("Inicio")]
    public KeyCode startKey = KeyCode.Space;
    public IntroVideo intro;
    public float fadeOutTime = 0.35f;

    [Header("Texto parpadeante")]
    public CanvasGroup promptGroup;    
    public float pulseSpeed = 3f;

    private bool started;

    void Awake()
    {
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

        if (promptGroup != null)
        promptGroup.alpha = Mathf.Lerp(0.45f, 1f, (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f);
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

        yield return null;

        if (player != null) player.enabled = true;

        if (startPanel != null) startPanel.SetActive(false);

        Debug.Log("StartScreen: juego iniciado");
    }
}