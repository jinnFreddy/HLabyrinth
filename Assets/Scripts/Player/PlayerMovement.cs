using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private MovementState state;
    [SerializeField] private Transform orientation;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float groundDrag;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;

    //[Header("Crouching")]
    //[SerializeField] private float crouchSpeed;
    //[SerializeField] private float crouchYScale;
    //[SerializeField] private float startYScale;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask isGround;
    private bool grounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Stamina")]
    [SerializeField] private Image staminaBar;
    [SerializeField] private float stamina;
    [SerializeField] private float staminaMaximum;
    [SerializeField] private float staminaRegenRate;
    [SerializeField] private float sprintStaminaCost;
    [SerializeField] private float jumpStaminaCost;
    [SerializeField] private bool isRecoveringFromSprint = false;
    [SerializeField] private float minStaminaForSprint = 25f;
    [SerializeField] private float staminaRecoveryDelay = 0.3f;

    [Header("Footsteps")]
    [SerializeField] private float footstepTimer = 0f;
    [SerializeField] private float walkStepInterval = 0.6f;
    [SerializeField] private float runStepInterval = 0.4f;
    [SerializeField] private bool hasGroundedBefore = false;

    [Header("Hurt Condition")]
    [SerializeField] public bool isSlowed;
    [SerializeField] private float hurtSpeedMultiplier;
    [SerializeField] private float hurtDuration;
    [SerializeField] private Material bloodiedMat;
    [SerializeField] private float intensity = 0.5f;
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseAmount = 0.15f;
    private float elapsed = 0f;
    private float recoveryDelayTimer = 0f;
    private Coroutine screenDamageTask;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        //startYScale = transform.localScale.y;
        stamina = staminaMaximum;
        staminaBar.color = Color.cyan;
        isSlowed = false;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, isGround);
        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded && (state != MovementState.sprinting))
        {
            recoveryDelayTimer += Time.deltaTime;
            if (recoveryDelayTimer > staminaRecoveryDelay)
            {
                stamina = Mathf.Min(stamina + staminaRegenRate * Time.deltaTime, staminaMaximum);
            }
        }
        else
        {
            recoveryDelayTimer = 0;
        }

            staminaBar.fillAmount = stamina / staminaMaximum;

        if (isSlowed)
        {
            staminaBar.color = Color.red;
        }
        else if (isRecoveringFromSprint)
        {
            staminaBar.color = Color.yellow;
        }
        else
        {
            staminaBar.color = Color.cyan;
        }
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    if (isSlowed) isSlowed = false;
        //    else if (!isSlowed) isSlowed = true;
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    GameManager.Instance.RestartPlaythrough();
        //}

        if (grounded) rb.linearDamping = groundDrag;
        else rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded && state != MovementState.crouching && stamina >= jumpStaminaCost && !isSlowed)
        {
            readyToJump = false;
            Jump();
            stamina -= jumpStaminaCost;
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //if (grounded && Input.GetKeyDown(crouchKey))
        //{
        //    transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        //    rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        //}

        //if (Input.GetKeyUp(crouchKey))
        //{
        //    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        //}
    }

    private void StateHandler()
    {
        moveSpeed = walkSpeed;

        // Crouching
        //if (Input.GetKey(crouchKey))
        //{
        //    state = MovementState.crouching;
        //    moveSpeed = crouchSpeed;
        //    return;
        //}

        // Hurt
        if (isSlowed)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed * hurtSpeedMultiplier;
            return;
        }

        // Sprinting
        bool wantsToSprint = Input.GetKey(sprintKey);
        bool hasEnoughStamina = stamina > minStaminaForSprint;
        bool canSprint = wantsToSprint && !isRecoveringFromSprint && hasEnoughStamina;
        if (canSprint)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;

            stamina -= sprintStaminaCost * Time.deltaTime;

            UpdateFootsteps(runStepInterval);

            if (stamina <= minStaminaForSprint)
            {
                isRecoveringFromSprint = true;
            }
        }

        // Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            UpdateFootsteps(walkStepInterval);

            if (isRecoveringFromSprint)
            {
                if (stamina >= staminaMaximum * 0.8f)
                {
                    isRecoveringFromSprint = false;
                }
            }
        }

        // Air
        else
        {
            state = MovementState.air;
            if (stamina <= minStaminaForSprint)
            {
                isRecoveringFromSprint = true;
            }
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            //if (rb.linearVelocity.y > 0)
            //{
            //    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            //}
        }

        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        } 

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.linearVelocity = horizontalVel + transform.up * jumpForce;
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public void HurtPlayer()
    {
        if (isSlowed)
        {
            KillPlayer();
        }
        else
        {
            isSlowed = true;
            ScreenDamageEffect(intensity);
            StartCoroutine(Recover());
        }
        
    }

    void ScreenDamageEffect(float intensity)
    {
        if(screenDamageTask != null)
        {
            StopCoroutine(screenDamageTask);
        }
        screenDamageTask = StartCoroutine(screenDamage(intensity));
    }

    private IEnumerator screenDamage(float intensity)
    {
        var targetRadius = Remap(intensity, 0, 1, 0.4f, -0.15f);
        var startRadius = 1f;

        for (float t = 0; t < 0.3f; t += Time.deltaTime)
        {
            float progress = t / 0.3f;
            float value = Mathf.Lerp(startRadius, targetRadius, progress);
            bloodiedMat.SetFloat("_Vignette_radius", value);
            yield return null;
        }

        while (elapsed < hurtDuration)
        {
            float pulse = Mathf.Sin(elapsed * pulseSpeed * Mathf.PI * 2) * pulseAmount;
            float radius = targetRadius + pulse;
            bloodiedMat.SetFloat("_Vignette_radius", radius);

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (float t = 0; t < 0.4f; t += Time.deltaTime)
        {
            float progress = t / 0.4f;
            float value = Mathf.Lerp(targetRadius, startRadius, progress);
            bloodiedMat.SetFloat("_Vignette_radius", value);
            yield return null;
        }

        bloodiedMat.SetFloat("_Vignette_radius", 1f);
    }

    IEnumerator Recover()
    {
        yield return new WaitForSeconds(hurtDuration);
        isSlowed = false;
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }

    private void KillPlayer()
    {
        GameManager.Instance.StartNewPlaythrough();
    }

    private void UpdateFootsteps(float stepInterval)
    {
        if (!hasGroundedBefore)
        {
            hasGroundedBefore = true;
            footstepTimer = stepInterval;
            return;
        }

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude < 0.5f) return; // Not actually moving

        footstepTimer += Time.deltaTime;

        if (footstepTimer >= stepInterval)
        {
            if (state == MovementState.sprinting)
            {
                SoundManager.PlaySoundWithPitch(SoundType.RUN, volume: 1f, pitch: 1.4f);
            }
            else if (state == MovementState.crouching)
            {
                SoundManager.PlaySoundWithPitch(SoundType.WALK, volume: .7f, pitch: .6f);
            }
            else
            {
                SoundManager.PlaySound(SoundType.WALK);
            }
            footstepTimer = 0f;
        }
    }
}
