using System.Collections;
using UnityEngine;

public class FireworksShow : MonoBehaviour
{
    [Header("Referencias")]
    public ParticleSystem burstPrefab;
    public Transform center;

    [Header("Espectáculo")]
    public int bursts = 12;
    public float showDuration = 5f;
    public Vector3 areaSize = new Vector3(12f, 4f, 6f);
    public float endPadding = 1.5f;

    [Header("Sonido (opcional)")]
    public AudioClip burstClip;
    [Range(0f, 1f)] public float volume = 0.8f;

    public IEnumerator Play()
    {
        if (burstPrefab == null) yield break;

        Transform c = center != null ? center : transform;
        float delay = showDuration / Mathf.Max(1, bursts);

        for (int i = 0; i < bursts; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-areaSize.x, areaSize.x) * 0.5f,
                Random.Range(-areaSize.y, areaSize.y) * 0.5f,
                Random.Range(-areaSize.z, areaSize.z) * 0.5f);

            Vector3 pos = c.position + offset;
            ParticleSystem ps = Instantiate(burstPrefab, pos, Quaternion.identity);

            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(
                Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.9f, 1f),
                Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.9f, 1f));

            ps.Play();
            if (burstClip != null) AudioSource.PlayClipAtPoint(burstClip, pos, volume);
            Destroy(ps.gameObject, main.duration + main.startLifetime.constantMax + 1f);

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(endPadding);
    }
}