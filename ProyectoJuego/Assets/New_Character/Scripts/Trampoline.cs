using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Impulso")]
    [Tooltip("Altura aproximada en unidades que alcanza el jugador al rebotar")]
    public float launchHeight = 7f;
    [Tooltip("Tiempo minimo entre rebotes, evita dobles activaciones")]
    public float cooldown = 0.25f;

    [Header("Sonido (opcional)")]
    public AudioClip bounceClip;
    [Range(0f, 1f)] public float bounceVolume = 0.8f;

    private float lastLaunchTime = -10f;

    void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastLaunchTime < cooldown) return;

        New_CharacterController player = other.GetComponent<New_CharacterController>();
        if (player == null) return;

        lastLaunchTime = Time.time;
        player.Launch(launchHeight);

        if (bounceClip != null)
            AudioSource.PlayClipAtPoint(bounceClip, Camera.main.transform.position, bounceVolume);
    }
}