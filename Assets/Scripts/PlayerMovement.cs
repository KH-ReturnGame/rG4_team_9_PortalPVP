using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveForce = 15f;
    public float jumpForce = 8f;
    public float maxSpeed = 6f;
    public float groundFriction = 20f; // how fast you decelerate when no input, while grounded

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
            rb.AddForce(Vector2.right * dir * moveForce);
        }
        else if (grounded)
        {
            // no input and on the ground: actively brake toward a stop
            // instead of coasting on whatever velocity we already had
            float newX = Mathf.MoveTowards(rb.linearVelocity.x, 0f, groundFriction * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }

        // hard cap: never exceed maxSpeed horizontally, regardless of input
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed);
        rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D c) { if (c.collider.CompareTag("Ground")) grounded = true; }
    void OnCollisionExit2D(Collision2D c) { if (c.collider.CompareTag("Ground")) grounded = false; }
}