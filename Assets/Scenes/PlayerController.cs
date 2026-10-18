
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpHeight = 4f;
    [SerializeField] float jumpBufferTime = 0.35f;
    [SerializeField] float coyoteTime = 0.15f;
    [SerializeField] float fallMultiplier = 2f;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer = ~0;

    [SerializeField] Transform visual;
    [SerializeField] float juiceDuration = 0.15f;
    [SerializeField]
    AnimationCurve juiceCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    Rigidbody rb;
    Collider[] playerColliders;

    bool isGrounded;
    bool wasGrounded;
    float h, v;
    float jumpBufferCounter;
    float coyoteTimeCounter;

    Vector3 visualStartScale;
    Coroutine juiceRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerColliders = GetComponentsInChildren<Collider>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (visual != null)
            visualStartScale = visual.localScale;
    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        wasGrounded = isGrounded;

        Vector3 forward = Camera.main != null
            ? Camera.main.transform.forward
            : Vector3.forward;

        Vector3 right = Camera.main != null
            ? Camera.main.transform.right
            : Vector3.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = Vector3.ClampMagnitude(
            forward * v + right * h, 1f
        ) * speed;

        rb.linearVelocity = new Vector3(
            move.x, rb.linearVelocity.y, move.z
        );

        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(
                Vector3.up * Physics.gravity.y * (fallMultiplier - 1f),
                ForceMode.Acceleration
            );
        }

        Vector3 checkPosition = groundCheck != null
            ? groundCheck.position
            : transform.position + Vector3.down * 0.9f;

        Collider[] hits = Physics.OverlapSphere(
            checkPosition,
            groundCheckRadius,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        isGrounded = false;

        foreach (Collider hit in hits)
        {
            bool isOwnCollider = false;

            foreach (Collider own in playerColliders)
            {
                if (hit == own)
                {
                    isOwnCollider = true;
                    break;
                }
            }

            if (isOwnCollider)
                continue;

            if (hit.ClosestPoint(checkPosition).y
                <= checkPosition.y + 0.05f)
            {
                isGrounded = true;
                break;
            }
        }

        if (rb.linearVelocity.y > 0.05f)
            isGrounded = false;

        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.fixedDeltaTime;

        if (isGrounded && !wasGrounded)
        {
            PlayJuice(0.8f, 1.2f);
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            float jumpImpulse = rb.mass * Mathf.Sqrt(
                2f * Mathf.Abs(Physics.gravity.y) * jumpHeight
            );

            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x, 0f, rb.linearVelocity.z
            );

            rb.AddForce(
                Vector3.up * jumpImpulse,
                ForceMode.Impulse
            );

            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            isGrounded = false;

            PlayJuice(1.3f, 0.8f);
        }
    }

    void PlayJuice(float stretch, float squash)
    {
        if (visual == null)
            return;

        if (juiceRoutine != null)
            StopCoroutine(juiceRoutine);

        juiceRoutine = StartCoroutine(
            AnimateJuice(stretch, squash)
        );
    }

    IEnumerator AnimateJuice(float stretch, float squash)
    {
        float elapsed = 0f;

        Vector3 effectScale = new Vector3(
            visualStartScale.x * squash,
            visualStartScale.y * stretch,
            visualStartScale.z * squash
        );

        while (elapsed < juiceDuration)
        {
            float t = elapsed / juiceDuration;
            float easedT = juiceCurve.Evaluate(t);

            visual.localScale = Vector3.Lerp(
                effectScale,
                visualStartScale,
                easedT
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        visual.localScale = visualStartScale;
        juiceRoutine = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 checkPosition = groundCheck != null
            ? groundCheck.position
            : transform.position + Vector3.down * 0.9f;

        Gizmos.DrawWireSphere(
            checkPosition,
            groundCheckRadius
        );
    }
}
