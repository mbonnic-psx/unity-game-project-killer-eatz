using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings")]
    public float walkingSpeed = 5f;
    public float runningSpeed = 8f;
    private float currentSpeed;
    public float jumpForce = 10f;
    public float gravity = -9.81f;
    public float coyoteTime = 0.2f;
    private float coyoteTimer;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private int maxDashCharges = 3; // Maximum number of charges
    [SerializeField] private float dashRechargeTime = 3f;
    private int currentDashCharges; // Current charges available
    private float rechargeTimer = 0f; // Timer to handle recharge
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    [Header("Camera Settings")]
    public float sensitivity = 2.0f;
    public float smoothing = 2.0f;
    public float lookXLimit = 45.0f;
    public float rotationX = 0;
    public Transform cameraTransform;
    public Camera playerCamera;
    private Vector2 smoothMouseDelta;

    [Header("Reference")]
    public GameObject player;
    private CharacterController controller;

    public Vector3 moveDirection = Vector3.zero;
    private bool canMove = true;
    private bool canRotate = true;
    private bool isInvincible;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isInvincible = false;

        currentDashCharges = maxDashCharges;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift) && dashCooldownTimer <= 0f && !isDashing && currentDashCharges > 0)
        {
            StartDash();
        }

        if (!isDashing)
        {
            PlayerController();
        }
        HandleDash();
        HandleDashRecharge();
        MouseCameraLook();

    }

    public void PlayerController()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (controller.isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && canMove && coyoteTimer > 0)
        {
            moveDirection.y = jumpForce;
            coyoteTimer = 0;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            moveDirection.x = Mathf.Lerp(moveDirection.x, Input.GetAxis("Horizontal") * currentSpeed, Time.deltaTime * 2f);
            moveDirection.z = Mathf.Lerp(moveDirection.z, Input.GetAxis("Vertical") * currentSpeed, Time.deltaTime * 2f);
        }

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        }

        // float targetFOV = isRunning ? 110f : 90f;
        // playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * 5f);

    }

    public void MouseCameraLook()
    {
        if (canRotate)
        {
            // Get the mouse input
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            // Smooth the mouse input
            smoothMouseDelta.x = Mathf.Lerp(smoothMouseDelta.x, mouseDelta.x, 1f / smoothing);
            smoothMouseDelta.y = Mathf.Lerp(smoothMouseDelta.y, mouseDelta.y, 1f / smoothing);

            // Update the camera and player rotations
            rotationX += -smoothMouseDelta.y * sensitivity;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            player.transform.Rotate(Vector3.up * smoothMouseDelta.x * sensitivity);

            // Clamp the vertical rotation to prevent upside-down camera
            float verticalRotation = transform.localEulerAngles.x;

            if (verticalRotation > 180)
            {
                verticalRotation -= 360;
            }

            verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

            transform.localEulerAngles = new Vector3(verticalRotation, transform.localEulerAngles.y, 0);

            //unlock the cursor by pressing Escape
            if (Input.GetKeyDown("escape"))
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    private void StartDash()
    {
        if (currentDashCharges <= 0) return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        dashDirection = moveDirection.normalized;
        dashDirection.z = 0;
        //dashDirection  = cameraTransform.forward.normalized;
        //dashDirection.y = 0;

        //Consume a charge
        currentDashCharges--;

        // Enable I-Frames
        isInvincible = true;
        Invoke("EndInvincibility", dashDuration / 2);
    }

    private void HandleDash()
    {
        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void HandleDashRecharge()
    {
        if (currentDashCharges < maxDashCharges)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer >= dashRechargeTime)
            {
                currentDashCharges++;
                rechargeTimer = 0f; // Reset the timer
            }
        }
    }

    private void EndInvincibility()
    {
        isInvincible = false;
    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;
    }

    public void EnableRotation(bool enable)
    {
        canRotate = enable;
    }
}
