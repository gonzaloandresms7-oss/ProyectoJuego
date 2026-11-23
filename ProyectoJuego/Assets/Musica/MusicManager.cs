using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public float fadeTime = 0.8f;

    private static MusicManager instance;
    private AudioSource src;
    private float baseVolume;

    public static MusicManager Instance
    {
        get
        {
            if (instance == null) instance = FindFirstObjectByType<MusicManager>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            var dupSrc = GetComponent<AudioSource>();
            if (dupSrc != null) dupSrc.Stop();
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        src = GetComponent<AudioSource>();
        baseVolume = src.volume;
    }

    public void PlayMusic()
    {
        src.mute = false;
        src.volume = baseVolume;
        if (!src.isPlaying) src.Play();
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        float start = src.volume;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, 0f, t / fadeTime);
            yield return null;
        }
        src.Stop();
        src.volume = baseVolume;
    }
}