using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class RunDust : MonoBehaviour
{
    public New_CharacterController player;
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        if (player == null)
            player = GetComponentInParent<New_CharacterController>();
    }

    void Update()
    {
        var emission = ps.emission;
        emission.enabled = player != null && player.IsSprinting && player.IsGrounded;
    }
}