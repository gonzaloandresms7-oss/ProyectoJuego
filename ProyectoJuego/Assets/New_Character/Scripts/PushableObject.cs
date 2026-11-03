using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    public float releaseDelay = 0.1f;
 
    private Rigidbody rb;
    private float unlockUntil;
    private bool locked;
 
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Lock();
    }
    public void Push()
    {
        unlockUntil = Time.time + releaseDelay;
        if (locked) Unlock();
    }

    void FixedUpdate()
    {
        if (!locked && Time.time > unlockUntil)
            Lock();
    }
 
    void Lock()
    {
        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(0f, v.y, 0f);
        rb.constraints = RigidbodyConstraints.FreezePositionX
                       | RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotation;
        locked = true;
    }
    void Unlock()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        locked = false;
    }
}
