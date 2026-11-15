using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class TimeSystem : MonoBehaviour
{
    [Header("Tiempo")]
    public float startTime=60f;
    public float maxTime=90f;
    public float timeBonus=5f;
    [Header("UI")]
    public TextMeshProUGUI timerText;
    public Color normalColor;
    public Color lowTimeColor;
    public float lowTimeThreshold = 10f;

    // NUEVO: el reloj que tiembla en los ultimos segundos
    [Header("Reloj")]
    public ClockShakeEffect clockShake;

    private float currentTime;
    private bool isRunning=true;
    private bool clockShaking = false;   // NUEVO
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = startTime;
        UpdateTimerUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            UpdateTimerUI();
            UpdateClockShake();   // NUEVO
            OnTimeOut();
            return;
        }

        UpdateTimerUI();
        UpdateClockShake();       // NUEVO
    }
    public void AddTime(float seconds)
    {
        currentTime = Mathf.Min(currentTime + seconds, maxTime);
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int totalSeconds = Mathf.CeilToInt(currentTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
        timerText.color = currentTime <= lowTimeThreshold ? lowTimeColor : normalColor;
    }

    // NUEVO: tiembla mientras quede poco tiempo y vuelve a su sitio si lo recuperas
    void UpdateClockShake()
    {
        if (clockShake == null) return;

        if (currentTime <= lowTimeThreshold)
        {
            clockShake.ShakeProgressive(currentTime, lowTimeThreshold);
            clockShaking = true;
        }
        else if (clockShaking)
        {
            // Recogiste un coleccionable y subiste del umbral: dejar de temblar
            clockShake.ResetPosition();
            clockShaking = false;
        }
    }

    void OnTimeOut()
    {
        Debug.Log("Se acabó el tiempo");
        if (clockShake != null) clockShake.ResetPosition();   // NUEVO
        SceneManager.LoadScene("SampleScene");
    }
}