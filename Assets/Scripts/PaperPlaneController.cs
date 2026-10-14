using UnityEngine;
using UnityEngine.InputSystem;

public class PaperPlaneController : MonoBehaviour
{
    public enum GameState
    {
        Ready,
        Playing,
        Ended
    }

    [Header("Launch")]
    [Range(1f, 30f)]
    public float initialThrowForce = 10f;

    [Range(-90f, 90f)]
    public float initialAngle = 15f;

    public float angleStep = 15f;

    [Header("Environment")]

    // Paper planes have no self-propulsion; tailwind provides sustained forward airflow.
    public float tailwindForce = 0.7f;

    [Header("Aerodynamics")]
    public float liftCoefficient = 0.09f;
    public float dragCoefficient = 0.025f;

    [Header("Stall")]
    public float stallAngle = 30f;
    public float stallSpeed = 5.5f;
    public float stallLiftMultiplier = 0.25f;
    public float stallDiveForce = 0.8f;

    [Header("Control")]
    public float pitchTorque = 1.8f;
    public float maxAngularVelocity = 120f;
    public float rotationAssist = 2f;

    [Header("Game End")]
    public float endDelay = 1f;

    [Header("Debug Info")]
    public float currentHeight;
    public float traveledDistance;
    public float currentSpeed;
    public float angleOfAttack;

    private Rigidbody2D rb;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float startX;

    private bool isGrounded = false;
    private float groundedTimer = 0f;

    private GameState currentState = GameState.Ready;

    void Start()
    {
        // Initialize plane state before gameplay starts
        rb = GetComponent<Rigidbody2D>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        rb.simulated = false;

        ApplyInitialAngle();
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Ready:
                HandleReadyInput();
                break;

            case GameState.Playing:
                CheckEndCondition();
                break;

            case GameState.Ended:
                HandleRetryInput();
                break;
        }
    }

    void FixedUpdate()
    {
        if (currentState != GameState.Playing)
            return;

        UpdateDebugInfo();

        // Apply aerodynamic simulation only while airborne
        if (!isGrounded)
        {
            bool isControlling = HandleFlightInput();

            ApplyTailwind();
            ApplyLift();
            ApplyDrag();

            // Natural passive alignment when player is not controlling pitch
            if (!isControlling)
            {
                AlignToVelocity();
            }
        }
    }

    void HandleReadyInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            initialAngle += angleStep;
            initialAngle = Mathf.Clamp(initialAngle, -90f, 90f);
            ApplyInitialAngle();
        }

        if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            initialAngle -= angleStep;
            initialAngle = Mathf.Clamp(initialAngle, -90f, 90f);
            ApplyInitialAngle();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        currentState = GameState.Playing;

        rb.simulated = true;
        startX = transform.position.x;

        // Initial launch impulse follows plane facing direction
        Vector2 launchDirection = transform.right;
        rb.AddForce(launchDirection * initialThrowForce, ForceMode2D.Impulse);
    }

    bool HandleFlightInput()
    {
        if (Keyboard.current == null) return false;

        float torque = 0f;
        bool isControlling = false;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            torque = pitchTorque;
            isControlling = true;
        }

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            torque = -pitchTorque;
            isControlling = true;
        }

        rb.AddTorque(torque);

        rb.angularVelocity = Mathf.Clamp(
            rb.angularVelocity,
            -maxAngularVelocity,
            maxAngularVelocity
        );

        return isControlling;
    }

    void ApplyTailwind()
    {
        // Constant horizontal wind to maintain playability
        rb.AddForce(Vector2.right * tailwindForce);
    }

    void ApplyLift()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.magnitude < 0.1f)
            return;

        Vector2 velocityDir = velocity.normalized;
        Vector2 forward = transform.right;

        // Angle of attack = plane direction vs actual movement direction
        angleOfAttack = Vector2.SignedAngle(forward, velocityDir);

        float speed = velocity.magnitude;

        // Simplified lift model
        float liftAmount = speed * liftCoefficient * Mathf.Cos(angleOfAttack * Mathf.Deg2Rad);

        bool isStalling = Mathf.Abs(angleOfAttack) > stallAngle && speed < stallSpeed;

        // Stall reduces lift and causes downward drop
        if (isStalling)
        {
            liftAmount *= stallLiftMultiplier;
            rb.AddForce(Vector2.down * stallDiveForce);
        }

        Vector2 liftDirection = Vector2.Perpendicular(velocityDir);

        rb.AddForce(liftDirection * liftAmount);
    }

    void ApplyDrag()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.magnitude < 0.1f)
            return;

        // Drag force opposes current movement
        Vector2 dragForce = -velocity.normalized * velocity.magnitude * dragCoefficient;

        rb.AddForce(dragForce);
    }

    void AlignToVelocity()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.magnitude < 0.2f)
            return;

        // Passive aerodynamic alignment
        float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationAssist * Time.fixedDeltaTime
        );
    }

    void CheckEndCondition()
    {
        if (!isGrounded)
        {
            groundedTimer = 0f;
            return;
        }

        // End game when plane remains grounded and almost stopped
        if (rb.linearVelocity.magnitude < 0.5f)
        {
            groundedTimer += Time.deltaTime;

            if (groundedTimer >= endDelay)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                currentState = GameState.Ended;
            }
        }
        else
        {
            groundedTimer = 0f;
        }
    }

    void HandleRetryInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Retry();
        }
    }

    void Retry()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        isGrounded = false;
        groundedTimer = 0f;
        currentState = GameState.Ready;

        ApplyInitialAngle();
    }

    void ApplyInitialAngle()
    {
        transform.rotation = Quaternion.Euler(0, 0, initialAngle);
    }

    void UpdateDebugInfo()
    {
        currentHeight = transform.position.y;
        traveledDistance = transform.position.x - startX;
        currentSpeed = rb.linearVelocity.magnitude;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 18;
        style.normal.textColor = Color.white;

        if (currentState == GameState.Ready)
        {
            GUI.Label(new Rect(20, 20, 700, 30), "Press W / Up: Increase launch angle", style);
            GUI.Label(new Rect(20, 50, 700, 30), "Press S / Down: Decrease launch angle", style);
            GUI.Label(new Rect(20, 80, 700, 30), "Current Launch Angle: " + initialAngle + "°", style);

            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 120, 400, 40), "Press Space to Start", style);
        }

        if (currentState == GameState.Playing)
        {
            GUI.Label(new Rect(20, 20, 700, 30), "Press W / Up: Pitch Up", style);
            GUI.Label(new Rect(20, 50, 700, 30), "Press S / Down: Dive", style);

            GUI.Label(new Rect(20, 100, 300, 30), "Height: " + currentHeight.ToString("F2") + " m", style);
            GUI.Label(new Rect(20, 130, 300, 30), "Distance: " + traveledDistance.ToString("F2") + " m", style);
            GUI.Label(new Rect(20, 160, 300, 30), "Speed: " + currentSpeed.ToString("F2") + " m/s", style);
        }

        if (currentState == GameState.Ended)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 60, 400, 40), "Flight Ended", style);
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 30, 400, 40), "Distance: " + traveledDistance.ToString("F2") + " m", style);
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 30, 400, 40), "Press R to Retry", style);
        }
    }
}