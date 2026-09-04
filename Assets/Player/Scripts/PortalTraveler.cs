using UnityEngine;

// Put this on the Torso (the root/reference body of the rig).
// It knows about every limb's Rigidbody2D and can move the whole
// group at once, preserving every joint's relative alignment so
// nothing snaps or stretches on teleport.
public class PortalTraveler : MonoBehaviour
{
    Rigidbody2D[] allBodies;
    Rigidbody2D root; // this object's own rigidbody = the reference point

    void Awake()
    {
        root = GetComponent<Rigidbody2D>();
        allBodies = GetComponentsInChildren<Rigidbody2D>();
    }

    // newRootPosition = where the torso should end up
    // rotationDeltaDegrees = how much to rotate the whole group (portal angle difference)
    public void TeleportGroup(Vector2 newRootPosition, float rotationDeltaDegrees)
    {
        Quaternion rot = Quaternion.Euler(0f, 0f, rotationDeltaDegrees);
        Vector2 oldRootPosition = root.position;

        foreach (var rb in allBodies)
        {
            Vector2 offsetFromRoot = rb.position - oldRootPosition;
            Vector2 rotatedOffset = rot * offsetFromRoot;

            rb.position = newRootPosition + rotatedOffset;
            rb.rotation += rotationDeltaDegrees;
            rb.linearVelocity = rot * rb.linearVelocity;
            // angularVelocity (spin) doesn't need rotating, it's not a directional vector
        }
    }
}
