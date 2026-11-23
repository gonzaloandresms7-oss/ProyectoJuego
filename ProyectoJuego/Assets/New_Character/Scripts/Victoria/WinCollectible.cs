using System.Collections;
using UnityEngine;

public class WinCollectible : MonoBehaviour
{
    [Header("Referencias")]
    public FireworksShow fireworks;
    public WinScreen winScreen;
    public TimeSystem timeSystem;
    public New_CharacterController player;

    [Header("Animación")]
    public float bobHeight = 0.25f;
    public float bobSpeed = 2f;

    [Header("Sonido (opcional)")]
    public AudioClip collectClip;

    [Header("Música de fondo")]
    public AudioSource musicSource;      
    public float musicFadeTime = 0.8f;

    private bool collected;
    private Vector3 startPos;

    void Start() { startPos = transform.position; }

    void Update()
    {
        if (collected) return;
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;
        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        var music = MusicManager.Instance;
        if (music != null) music.FadeOut();
        else Debug.LogWarning("WinCollectible: no hay MusicManager en la escena");
        WinScreen.InProgress = true;

        StartCoroutine(FadeOutMusic());

        if (timeSystem != null) timeSystem.StopTimer();
        if (player != null) player.enabled = false;

        if (collectClip != null)
            AudioSource.PlayClipAtPoint(collectClip, transform.position);

        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;

        if (fireworks != null) yield return fireworks.Play();

        if (winScreen != null) winScreen.Show();
    }

    IEnumerator FadeOutMusic()
    {
        if (musicSource == null) yield break;

        float startVolume = musicSource.volume;
        float t = 0f;
        while (t < musicFadeTime)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / musicFadeTime);
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = startVolume;   
    }
}