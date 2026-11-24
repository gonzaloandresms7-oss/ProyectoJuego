using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]
public class IntroVideo : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject videoPanel;
    public RawImage videoImage;

    [Header("Saltar")]
    public KeyCode skipKey = KeyCode.Return;
    public float skipDelay = 0.5f;

    [Header("Opciones")]
    public bool playOnlyOnce = true;
    public float prepareTimeout = 5f;
    public float stallTimeout = 3f;

    public bool IsPlaying { get; private set; } = true;
    public int FinishedFrame { get; private set; } = -1;
    public bool BlocksStart => IsPlaying || Time.frameCount == FinishedFrame;

    private static bool alreadyPlayed;

    private VideoPlayer vp;
    private AudioSource audioSrc;
    private RenderTexture rt;
    private float startTime;
    private bool ended;
    private bool playing;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { alreadyPlayed = false; }

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        audioSrc = GetComponent<AudioSource>();

        if (playOnlyOnce && alreadyPlayed)
        {
            audioSrc.playOnAwake = false;
            audioSrc.Stop();
            vp.playOnAwake = false;
            vp.Stop();
            if (videoPanel != null) videoPanel.SetActive(false);
            IsPlaying = false;
            enabled = false;
            return;
        }

        audioSrc.playOnAwake = false;
        audioSrc.ignoreListenerPause = true;

        vp.playOnAwake = false;
        vp.isLooping = false;
        vp.waitForFirstFrame = true;
        vp.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
        vp.controlledAudioTrackCount = 1;
        vp.EnableAudioTrack(0, true);
        vp.SetTargetAudioSource(0, audioSrc);

        vp.errorReceived += OnError;
        vp.loopPointReached += OnFinished;
    }

    IEnumerator Start()
    {
        if (videoPanel != null)
        {
            videoPanel.SetActive(true);
            videoPanel.transform.SetAsLastSibling();
        }

        bool noVideo = vp.clip == null && string.IsNullOrEmpty(vp.url);
        if (noVideo || (playOnlyOnce && alreadyPlayed))
        {
            End();
            yield break;
        }
        alreadyPlayed = true;

        int w = vp.clip != null ? (int)vp.clip.width : 0;
        int h = vp.clip != null ? (int)vp.clip.height : 0;
        if (w <= 0 || h <= 0) { w = 1920; h = 1080; }

        rt = new RenderTexture(w, h, 0);
        vp.targetTexture = rt;
        if (videoImage != null) videoImage.texture = rt;

        vp.Prepare();
        float waited = 0f;
        while (!vp.isPrepared && !ended)
        {
            waited += Time.unscaledDeltaTime;
            if (waited > prepareTimeout)
            {
                Debug.LogWarning("IntroVideo: el video no se pudo preparar (codec no compatible?). Se omite.");
                End();
                yield break;
            }
            yield return null;
        }
        if (ended) yield break;

        startTime = Time.realtimeSinceStartup;
        vp.Play();
        playing = true;
    }

    void Update()
    {
        if (ended) return;

        if (!playing) return;

        if (Time.realtimeSinceStartup - startTime > stallTimeout && vp.frame <= 0)
        {
            Debug.LogWarning("IntroVideo: el video no avanza (pantalla negra). Se omite.");
            End();
            return;
        }

        if (Time.realtimeSinceStartup - startTime >= skipDelay && Input.GetKeyDown(skipKey))
            End();
    }

    void OnFinished(VideoPlayer source) { End(); }

    void OnError(VideoPlayer source, string message)
    {
        Debug.LogWarning("IntroVideo: error al reproducir el video: " + message);
        End();
    }

    void End()
    {
        if (ended) return;
        ended = true;

        if (vp.isPlaying) vp.Stop();
        if (videoImage != null) videoImage.texture = null;
        if (videoPanel != null) videoPanel.SetActive(false);

        vp.targetTexture = null;
        if (rt != null) { rt.Release(); Destroy(rt); rt = null; }

        IsPlaying = false;
        FinishedFrame = Time.frameCount;
    }
}