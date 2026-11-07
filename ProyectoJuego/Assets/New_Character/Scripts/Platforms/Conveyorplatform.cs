using UnityEngine;

public class Conveyorplatform : MonoBehaviour
{
    [Header("Impulso")]
    public Vector3 localDirection = Vector3.forward;
    public float pushSpeed = 4f;

    Vector3 WorldDirection()
    {
        Vector3 d = transform.TransformDirection(localDirection);
        d.y = 0f;                  
        return d.normalized;
    }
 
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
 
        New_CharacterController player = other.GetComponent<New_CharacterController>();
        if (player != null)
            player.SetPlatformPush(WorldDirection() * pushSpeed);
    }
 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 start = transform.position + Vector3.up * 0.2f;
        Vector3 end = start + WorldDirection() * 2f;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, 0.15f);
    }
}
