using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class LiveSystem : MonoBehaviour
{
    public Transform respawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (respawnPoint == null)
        {
            respawnPoint = this.transform;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            CharacterController cc = GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;
            transform.position = respawnPoint.position;
            if (cc != null) cc.enabled = true;
        }
    }
}
