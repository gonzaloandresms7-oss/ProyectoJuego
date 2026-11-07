using UnityEngine;
[RequireComponent (typeof(CharacterController))]
public class New_CharacterController : MonoBehaviour
{
    [Header("Movimiento")]
    public float WalkSpeed=4f;
    public float SprintSpeed=6f;
    public float jumpHeight =2f;    
    public float rotationSpeed=10f;
    public float mouseSensivity=1f;
    public float gravity=-20f;

    [Header("Doble salto")]
    public int maxAirJumps = 1;
    public float airJumpHeight = 1.6f;
    public LayerMask groundMask = ~0;
    private int airJumpsLeft;
    public float landingBlockMaxDistance = 0.4f;

    [Header("Referenciación")]
    public Transform cameraTransform;
    public Animator animator;

    //puse jumpbuffer y coyotetime para hacer más fluido la animación
    [Header("Salto fluido")]
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.2f;
    public float groundedStickForce = -5f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    
    private CharacterController characterController;
    private Vector3 Velocity;
    private float currentSpeed;
    private float yaw;
    private Vector3 externalVelocity=Vector3.zero;

    public bool IsMoving{get;private set;}
    public Vector2 CurrentInput{get;private set;}
    public bool IsGrounded{get;private set;}
    public float CurrentYaw => yaw;

    //Codigo nuevo
    [Header("Empujar objetos")]
    public KeyCode pushKey = KeyCode.E;
    public float pushPower = 2f;
    public float maxPushSpeed = 2.5f;
    public float pushMoveMultiplier = 0.6f;
    public GameObject pushPrompt;
    private float lastPushableTouchTime = -10f;
    public bool IsPushing { get; private set; }

    //Agregar sonido al salto y doble salto
    [Header("Sonidos")]
    public AudioSource sfxSource;
    public AudioClip jumpClip;
    public AudioClip doubleJumpClip;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    //Variables para el movimiento en la banda transportadora
    private Vector3 platformPush;
    private float platformPushTime = -10f;


    void OnControllerColliderHit(ControllerColliderHit hit)
{
    Rigidbody body = hit.collider.attachedRigidbody;
    if (body == null || body.isKinematic) return;

    if (hit.moveDirection.y < -0.3f) return;

    // Contacto con un objeto empujable (sirve para el aviso y la animación)
    lastPushableTouchTime = Time.time;

    // Solo empujar si se mantiene la tecla y el jugador se está moviendo
    if (!IsMoving || !Input.GetKey(pushKey)) return;

    PushableObject pushable = body.GetComponent<PushableObject>();
    if (pushable != null) pushable.Push();

    Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z).normalized;

    float massFactor = 1f / Mathf.Max(1f, body.mass);
    Vector3 targetVelocity = pushDir * pushPower * massFactor * currentSpeed;
    targetVelocity = Vector3.ClampMagnitude(targetVelocity, maxPushSpeed);

    Vector3 current = body.linearVelocity;  
    body.linearVelocity = new Vector3(targetVelocity.x, current.y, targetVelocity.z);
}

    void Start()
    {
        characterController=GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        UpdateAnimator();
        UpdatePushPrompt();
    }

    //Para ayudar a que el doble salto tenga timebuffer
    bool WouldLandSoon()
{
    if (Velocity.y >= 0f) return false;

    float checkDistance = Mathf.Min(Mathf.Abs(Velocity.y) * jumpBufferTime, landingBlockMaxDistance);

    Vector3 feet = transform.position + characterController.center
                   - Vector3.up * (characterController.height * 0.5f);

    return Physics.Raycast(feet + Vector3.up * 0.1f, Vector3.down,
                           checkDistance + 0.1f, groundMask,
                           QueryTriggerInteraction.Ignore);
}

    void HandleMovement()
{
    IsGrounded = characterController.isGrounded;

    if (IsGrounded)
    {
        coyoteTimeCounter = coyoteTime;
        airJumpsLeft=maxAirJumps;
    }
    else
        coyoteTimeCounter -= Time.deltaTime;

    if (Input.GetButtonDown("Jump"))
        jumpBufferCounter = jumpBufferTime;
    else
        jumpBufferCounter -= Time.deltaTime;

    if (IsGrounded && Velocity.y < 0)
    {
        if (externalVelocity.y > -0.05f && externalVelocity.y < 0.05f)
            Velocity.y = groundedStickForce;
        else
            Velocity.y = -2f;
    }

    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");
    Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
    IsMoving = inputDirection.magnitude > 0.1f;
    IsPushing = IsMoving && IsGrounded && Input.GetKey(pushKey) && Time.time - lastPushableTouchTime < 0.15f;

    Vector3 moveDirection = Vector3.zero;

   if (IsMoving)
{
    moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * inputDirection;

    if (IsPushing)
    {
        currentSpeed = WalkSpeed * pushMoveMultiplier;
    }
    else
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isSprinting ? SprintSpeed : WalkSpeed;
    }
}

    //Añadir doble salto
    if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
    {
        Velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        animator?.SetBool("IsJumping", true);
        PlaySFX(jumpClip);

        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }
    else if (Input.GetButtonDown("Jump") && !IsGrounded && coyoteTimeCounter <= 0f && airJumpsLeft > 0 && !WouldLandSoon())
    {
        Velocity.y = Mathf.Sqrt(airJumpHeight * -2f * gravity);
        airJumpsLeft--;
        jumpBufferCounter = 0f;
        PlaySFX(doubleJumpClip);

        if (animator != null)
        {
            animator.SetBool("IsJumping", true);
            animator.SetTrigger("DoubleJump");
        }
    }
    Velocity.y += gravity * Time.deltaTime;
    Vector3 push = (Time.time - platformPushTime < 0.1f) ? platformPush : Vector3.zero;
    Vector3 finalMovement = (moveDirection * currentSpeed + externalVelocity + push) * Time.deltaTime;
    finalMovement.y += Velocity.y * Time.deltaTime;

    characterController.Move(finalMovement);

    if (IsGrounded && Velocity.y < 0f)
    {
        animator?.SetBool("IsJumping", false);
    }
}

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X")*mouseSensivity;
        yaw += mouseX;

        if(IsMoving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yaw, 0f), rotationSpeed*Time.deltaTime);

        }
    }
    
  
void UpdateAnimator()
{
    float SpeedPercent = IsMoving ? (currentSpeed == SprintSpeed ? 1f : 0.5f) : 0f;

    animator?.SetFloat("Speed", SpeedPercent, 0.1f, Time.deltaTime);
    animator?.SetBool("IsGrounded", IsGrounded);
    animator?.SetFloat("VerticalSpeed", Velocity.y);
    animator.SetBool("IsPushing", IsPushing);
}

void UpdatePushPrompt()
{
    if (pushPrompt == null) return;

    bool touching = Time.time - lastPushableTouchTime < 0.15f;
    pushPrompt.SetActive(touching);
}

public void SetExternalVelocity(Vector3 platformVelocity)
    {
        externalVelocity=platformVelocity;
    }

//Metodo nuevo para el sonido cuando salte 
void PlaySFX(AudioClip clip)
{
    if (clip != null && sfxSource != null)
        sfxSource.PlayOneShot(clip, sfxVolume);
}

//Metodo para la plataforma que empuja
public void SetPlatformPush(Vector3 velocity)
{
    platformPush = velocity;
    platformPushTime = Time.time;
}
}

