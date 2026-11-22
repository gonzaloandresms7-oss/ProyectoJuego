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

    [Header("Reloj")]
    public ClockShakeEffect clockShake;

    [Header("Game Over")]
    public GameOverScreen gameOver;

    private float currentTime;
    private bool isRunning=true;
    private bool clockShaking = false;   
    void Start()
    {
        currentTime = startTime;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            UpdateTimerUI();
            UpdateClockShake();   
            OnTimeOut();
            return;
        }

        UpdateTimerUI();
        UpdateClockShake();       
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
            clockShake.ResetPosition();
            clockShaking = false;
        }
    }

    void OnTimeOut()
    {
        Debug.Log("Se acabó el tiempo");
        if (clockShake != null) clockShake.ResetPosition();  

        if (gameOver != null)
        {
            gameOver.Show();
            return;
        }

        SceneManager.LoadScene("SampleScene");
    }
}