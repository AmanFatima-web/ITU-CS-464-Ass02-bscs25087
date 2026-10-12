
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpHeight = 4f;
    [SerializeField] float jumpBufferTime = 0.35f;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer = ~0;

    Rigidbody rb;
    Collider[] playerColliders;
    bool isGrounded;
    float h, v;
    float jumpBufferCounter;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerColliders = GetComponentsInChildren<Collider>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
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

            if (hit.ClosestPoint(checkPosition).y <= checkPosition.y + 0.05f)
            {
                isGrounded = true;
                break;
            }
        }

        if (rb.linearVelocity.y > 0.05f)
            isGrounded = false;

        if (jumpBufferCounter > 0f && isGrounded)
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
            isGrounded = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 checkPosition = groundCheck != null
            ? groundCheck.position
            : transform.position + Vector3.down * 0.9f;

        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }
}
