using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Video de introduccion: se reproduce apenas empieza la escena, se puede saltar con una tecla
// y al terminar (o saltarlo) deja paso a StartScreen (el logo de "Presiona ESPACIO").
// Ponlo en un objeto vacio que NO sea el panel del video. Unity agrega solo el Video Player y el Audio Source.
[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]
public class IntroVideo : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Panel negro de pantalla completa que contiene el RawImage")]
    public GameObject videoPanel;
    [Tooltip("RawImage donde se dibuja el video")]
    public RawImage videoImage;

    [Header("Saltar")]
    public KeyCode skipKey = KeyCode.Return;
    [Tooltip("Segundos antes de poder saltar (evita saltarlo sin querer)")]
    public float skipDelay = 0.5f;

    [Header("Opciones")]
    [Tooltip("Si el juego recarga la escena (al acabarse el tiempo), no repetir el video")]
    public bool playOnlyOnce = true;

    // StartScreen consulta esto: mientras sea true, no acepta la tecla de inicio
    public bool IsPlaying { get; private set; } = true;
    public int FinishedFrame { get; private set; } = -1;
    public bool BlocksStart => IsPlaying || Time.frameCount == FinishedFrame;

    private static bool alreadyPlayed;

    private VideoPlayer vp;
    private AudioSource audioSrc;
    private RenderTexture rt;
    private float startTime;
    private bool ended;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { alreadyPlayed = false; }

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();
        audioSrc = GetComponent<AudioSource>();

        // El audio del video debe sonar aunque StartScreen pause el audio del juego
        audioSrc.playOnAwake = false;
        audioSrc.ignoreListenerPause = true;

        vp.playOnAwake = false;
        vp.isLooping = false;
        vp.waitForFirstFrame = true;
        vp.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;   // no depende de Time.timeScale (que vale 0 en el inicio)
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
            videoPanel.transform.SetAsLastSibling();   // por encima de la pantalla del logo
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

        // Preparar antes de reproducir evita ver un parpadeo negro al inicio
        vp.Prepare();
        while (!vp.isPrepared && !ended) yield return null;
        if (ended) yield break;

        startTime = Time.realtimeSinceStartup;
        vp.Play();
    }

    void Update()
    {
        if (ended) return;

        if (Time.realtimeSinceStartup - startTime >= skipDelay && Input.GetKeyDown(skipKey))
            End();
    }

    void OnFinished(VideoPlayer source) { End(); }

    void OnError(VideoPlayer source, string message)
    {
        // Si el video falla (codec no compatible, etc.) no dejar el juego atascado en negro
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