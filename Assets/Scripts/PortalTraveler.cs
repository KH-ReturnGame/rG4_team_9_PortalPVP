using UnityEngine;

// Put this on the player object itself (the one with the Rigidbody2D
// and the sprite). Handles moving that single object through a portal:
// position, optional visual rotation, and velocity redirect.
public class PortalTraveler : MonoBehaviour
{
    [Tooltip("If true, the sprite rotates to match the portal's orientation delta. If false, the sprite stays upright and only velocity direction is redirected - use this for a standing humanoid sprite.")]
    public bool rotateVisualOrientation = false;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // newPosition = where this object should end up
    // rotationDeltaDegrees = angle difference between the two portals
    public void Teleport(Vector2 newPosition, float rotationDeltaDegrees)
    {
        Quaternion rot = Quaternion.Euler(0f, 0f, rotationDeltaDegrees);

        rb.position = newPosition;

        if (rotateVisualOrientation)
            rb.rotation += rotationDeltaDegrees;
        // else: leave rb.rotation untouched, sprite stays upright

        rb.linearVelocity = rot * rb.linearVelocity;
        // angularVelocity (spin) doesn't need rotating, it's not a directional vector
    }
}