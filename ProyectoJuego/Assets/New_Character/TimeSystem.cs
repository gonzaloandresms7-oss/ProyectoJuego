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

    private float currentTime;
    private bool isRunning=true;
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
            OnTimeOut();
            return;
        }

        UpdateTimerUI();
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

    void OnTimeOut()
    {
        Debug.Log("Se acabó el tiempo");
        SceneManager.LoadScene("SampleScene");
    }
}
