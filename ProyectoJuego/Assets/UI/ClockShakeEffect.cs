using UnityEngine;

public class ClockShakeEffect : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalPosition;

    [Header("Configuración del Shake Progresivo")]
    [Tooltip("Intensidad inicial cuando empiezan los últimos segundos")]
    public float minMagnitude = 2f;
    [Tooltip("Intensidad máxima cuando el tiempo llega a 0")]
    public float maxMagnitude = 18f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    // CAMBIO: ahora recibe el umbral (por defecto 10) para que coincida con TimeSystem.lowTimeThreshold
    public void ShakeProgressive(float remainingTime, float threshold = 10f)
    {
        float clampedTime = Mathf.Clamp(remainingTime, 0f, threshold);

        float t = 1f - (clampedTime / threshold);

        float currentMagnitude = Mathf.Lerp(minMagnitude, maxMagnitude, t);

        // Generar desplazamiento aleatorio
        float x = Random.Range(-1f, 1f) * currentMagnitude;
        float y = Random.Range(-1f, 1f) * currentMagnitude;

        rectTransform.anchoredPosition = originalPosition + new Vector3(x, y, 0);
    }


    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }
}