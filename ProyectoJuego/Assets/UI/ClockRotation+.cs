using UnityEngine;

public class ClockRotation : MonoBehaviour
{
    [Tooltip("Velocidad de rotación en grados por segundo")]
    public float rotationSpeed = -45f; // Negativo para que gire hacia la derecha (horario)

    void Update()
    {
        // Rota la imagen sobre el eje Z de forma constante
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}