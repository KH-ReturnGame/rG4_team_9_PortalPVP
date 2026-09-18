using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveForce = 15f;
    public float jumpForce = 8f;
    public float MaxSpeedX = 20f;
    public float MaxSpeedY = 20f;
    public float groundFriction = 20f; // how fast you decelerate when no input, while grounded

    [Range(0f, 1f)]
    public float airControlMultiplier = 0.15f; // 0 = no air control at all, 1 = same as ground

    [Header("Controls")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.W;

    Rigidbody2D rb;
    bool grounded;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Update()
    {
        if (Input.GetKeyDown(jumpKey) && grounded)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        float dir = 0f;
        if (Input.GetKey(leftKey)) dir -= 1f;
        if (Input.GetKey(rightKey)) dir += 1f;

        if (dir != 0f)
        {
            // full force on the ground, heavily reduced force in the air
            float appliedForce = moveForce * (grounded ? 1f : airControlMultiplier);
            rb.AddForce(Vector2.right * dir * appliedForce);
        }
        else if (grounded)
        {
            // no input and on the ground: actively brake toward a stop
            // instead of coasting on whatever velocity we already had
            float newX = Mathf.MoveTowards(rb.linearVelocity.x, 0f, groundFriction * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }
        // note: no braking at all while airborne and no input - momentum
        // carries naturally, which is usually what you want mid-jump

        // hard cap: never exceed MaxSpeed in either axis, regardless of input
        float clampedY = Mathf.Clamp(rb.linearVelocity.y, -MaxSpeedY, MaxSpeedY);
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -MaxSpeedX, MaxSpeedX);
        rb.linearVelocity = new Vector2(clampedX, clampedY);
    }

    void OnCollisionEnter2D(Collision2D c) { if (c.collider.CompareTag("Ground")) grounded = true; }
    void OnCollisionExit2D(Collision2D c) { if (c.collider.CompareTag("Ground")) grounded = false; }
}