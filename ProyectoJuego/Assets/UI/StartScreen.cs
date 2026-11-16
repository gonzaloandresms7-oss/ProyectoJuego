using System.Collections;
using TMPro;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Panel de la pantalla de inicio (el que tiene el logo)")]
    public GameObject startPanel;
    [Tooltip("Opcional: CanvasGroup del panel, para que se desvanezca suavemente")]
    public CanvasGroup panelGroup;
    [Tooltip("El jugador (para bloquear sus controles hasta empezar)")]
    public New_CharacterController player;

    [Header("Inicio")]
    public KeyCode startKey = KeyCode.Space;
    public float fadeOutTime = 0.35f;

    [Header("Texto parpadeante (opcional)")]
    public TextMeshProUGUI promptText;
    public float pulseSpeed = 3f;

    private bool started;

    void Awake()
    {
     
        Time.timeScale = 0f;

        if (startPanel != null) startPanel.SetActive(true);
        if (player != null) player.enabled = false;   
    }

    void Update()
    {
        if (started) return;

        if (promptText != null)
        {
            float a = Mathf.Lerp(0.45f, 1f, (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f);
            Color c = promptText.color;
            c.a = a;
            promptText.color = c;
        }

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
        }

        if (startPanel != null) startPanel.SetActive(false);

        Time.timeScale = 1f;

        yield return null;

        if (player != null) player.enabled = true;
    }
}