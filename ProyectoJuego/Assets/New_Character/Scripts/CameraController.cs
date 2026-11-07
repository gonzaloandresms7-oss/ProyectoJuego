using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform player;          
    public Transform cameraTarget;    

    [Header("Encuadre")]
    public float distance = 9f;
    public float focusHeight = 1f;
    [Range(-10f, 30f)] public float pitch = 6f;
    [Range(-180f, 180f)] public float yaw = -90f;
    public float fieldOfView = 45f;

    [Header("Seguimiento suave")]
    public float horizontalSmoothTime = 0.25f;
    public float verticalSmoothTime = 0.4f;   

    [Header("Anticipacion (look ahead)")]
    public float lookAheadFactor = 0.6f;
    public float maxLookAhead = 2f;
    public float lookAheadSmoothTime = 0.6f;

    [Header("Respawn")]
    public float snapDistance = 20f;

    private Camera cam;
    private Transform mainCamera;
    private Vector3 lastTargetPos;
    private Vector3 lookAhead;
    private Vector3 lookAheadVelocity;
    private float velX, velY, velZ;

    void Start()
    {
        cam = Camera.main;
        mainCamera = cam.transform;

        if (cameraTarget == null) cameraTarget = player;

        lastTargetPos = cameraTarget.position;
        mainCamera.rotation = Quaternion.Euler(pitch, yaw, 0f);
        mainCamera.position = GetDesiredPosition();
        cam.fieldOfView = fieldOfView;
    }

    void LateUpdate()
    {
        if (cameraTarget == null) return;

        cam.fieldOfView = fieldOfView;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        UpdateLookAhead();

        Vector3 desiredPos = GetDesiredPosition();
        Vector3 pos = mainCamera.position;

        if ((desiredPos - pos).sqrMagnitude > snapDistance * snapDistance)
        {
            pos = desiredPos;
            velX = velY = velZ = 0f;
            lookAhead = Vector3.zero;
            lookAheadVelocity = Vector3.zero;
        }
        else
        {
            pos.x = Mathf.SmoothDamp(pos.x, desiredPos.x, ref velX, horizontalSmoothTime);
            pos.y = Mathf.SmoothDamp(pos.y, desiredPos.y, ref velY, verticalSmoothTime);
            pos.z = Mathf.SmoothDamp(pos.z, desiredPos.z, ref velZ, horizontalSmoothTime);
        }

        mainCamera.position = pos;
        mainCamera.rotation = rotation;   
    }

    void UpdateLookAhead()
    {
        Vector3 delta = cameraTarget.position - lastTargetPos;
        lastTargetPos = cameraTarget.position;

        Vector3 flatVelocity = new Vector3(delta.x, 0f, delta.z) / Mathf.Max(Time.deltaTime, 0.0001f);
        Vector3 desired = Vector3.ClampMagnitude(flatVelocity * lookAheadFactor, maxLookAhead);

        lookAhead = Vector3.SmoothDamp(lookAhead, desired, ref lookAheadVelocity, lookAheadSmoothTime);
    }

    Vector3 GetDesiredPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focus = cameraTarget.position + lookAhead + Vector3.up * focusHeight;
        return focus - rotation * Vector3.forward * distance;
    }
}
