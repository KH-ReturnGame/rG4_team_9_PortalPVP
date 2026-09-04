using UnityEngine;

public class TorsoMover : MonoBehaviour
{
    public float moveForce = 15f;
    public float jumpForce = 8f;

    Rigidbody2D rb;
    bool grounded;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && grounded)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        float dir = 0f;
        if (Input.GetKey(KeyCode.A)) dir -= 1f;
        if (Input.GetKey(KeyCode.D)) dir += 1f;
        rb.AddForce(Vector2.right * dir * moveForce);
    }

    void OnCollisionEnter2D(Collision2D c) { if (c.collider.CompareTag("Ground")) grounded = true; }
    void OnCollisionExit2D(Collision2D c) { if (c.collider.CompareTag("Ground")) grounded = false; }
}