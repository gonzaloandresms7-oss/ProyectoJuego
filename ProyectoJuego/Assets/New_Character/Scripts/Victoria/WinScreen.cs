using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject winPanel;
    public CanvasGroup panelGroup;

    [Header("Opciones")]
    public float fadeInTime = 0.6f;
    public float showTime = 4f;           
    public AudioClip winClip;
    [Range(0f, 1f)] public float clipVolume = 0.9f;

    public static bool InProgress { get; set; }
    public static bool IsShown { get; private set; }

    void Start()
    {
        InProgress = false;
        IsShown = false;
        if (winPanel != null) winPanel.SetActive(false);
    }

    public void Show()
    {
        if (IsShown) return;
        IsShown = true;
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            winPanel.transform.SetAsLastSibling();
        }

        if (winClip != null)
            AudioSource.PlayClipAtPoint(winClip, Camera.main.transform.position, clipVolume);

        if (panelGroup != null && fadeInTime > 0f)
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

        yield return new WaitForSecondsRealtime(showTime);

        InProgress = false;
        IsShown = false;
        StartScreen.ShowOnNextLoad();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}