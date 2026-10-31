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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController=GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        UpdateAnimator();

    }

    //Para ayudar a que el doble salto tenga timebuffer
    bool WouldLandSoon()
{
    if (Velocity.y >= 0f) return false;

    float checkDistance = Mathf.Abs(Velocity.y) * jumpBufferTime;

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

    Vector3 moveDirection = Vector3.zero;

    if (IsMoving)
    {
        moveDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * inputDirection;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isSprinting ? SprintSpeed : WalkSpeed;
    }

    //Añadir doble salto
    if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
    {
        Velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        animator?.SetBool("IsJumping", true);

        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }
    else if (Input.GetButtonDown("Jump") && !IsGrounded && coyoteTimeCounter <= 0f && airJumpsLeft > 0 && !WouldLandSoon())
    {
        Velocity.y = Mathf.Sqrt(airJumpHeight * -2f * gravity);
        airJumpsLeft--;
        jumpBufferCounter = 0f;

        if (animator != null)
        {
            animator.SetBool("IsJumping", true);
        }
    }
    Velocity.y += gravity * Time.deltaTime;

    Vector3 finalMovement = (moveDirection * currentSpeed + externalVelocity) * Time.deltaTime;
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
}

public void SetExternalVelocity(Vector3 platformVelocity)
    {
        externalVelocity=platformVelocity;
    }
}
