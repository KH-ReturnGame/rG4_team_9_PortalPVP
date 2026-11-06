using UnityEngine;

/// <summary>
/// One of these goes on EACH player. Set the keys differently in the Inspector:
///   Player 1: Left=A, Right=D, Fire=W, ToggleMode=S, SwapColor=LeftShift
///   Player 2: Left=LeftArrow, Right=RightArrow, Fire=UpArrow, ToggleMode=DownArrow, SwapColor=RightShift
/// </summary>
public class PortalGun : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode aimLeftKey = KeyCode.A;
    public KeyCode aimRightKey = KeyCode.D;
    public KeyCode fireKey = KeyCode.W;
    public KeyCode toggleModeKey = KeyCode.S;
    public KeyCode swapColorKey = KeyCode.LeftShift;

    [Header("Movement")]
    public PlayerMovement movement;
    public bool lockHorizontalInPortalMode = true;  // needed if you aim with A/D
    public bool lockJumpInPortalMode = false;       // leave false if fire is a different key than jump

    [Header("Aiming")]
    public Transform muzzle;               // empty child object where the shot starts
    public float aimSpeed = 120f;          // degrees per second
    public float startAngle = 90f;         // 0 = right, 90 = up
    public float maxDistance = 50f;

    [Header("Layers")]
    public LayerMask hitMask;              // everything the shot can hit (walls, floors, etc.)
    public LayerMask portalSurfaceMask;    // only the surfaces portals are allowed to stick to

    [Header("Portals")]
    public Transform bluePortal;
    public Transform orangePortal;
    public float surfaceOffset = 0.02f;    // pushes portal slightly off the wall to avoid z-fighting/clipping
    public Vector3 portalFacingAxis = Vector3.up; // which local axis of your portal sprite points "out" of the wall

    [Header("Optional visuals")]
    public LineRenderer aimLine;           // shows the aim direction in portal mode
    public Transform aimPivot;             // optional: something (like an arm/arrow sprite) that rotates with the aim

    // State
    public bool InPortalMode { get; private set; }
    public bool BlueSelected { get; private set; } = true;

    float aimAngle;

    void Start()
    {
        aimAngle = startAngle;
        if (aimLine != null) aimLine.enabled = false;
        if (muzzle == null) muzzle = transform;
    }

    void Update()
    {
        // Tap to switch between movement mode and portal gun mode
        if (Input.GetKeyDown(toggleModeKey))
            SetPortalMode(!InPortalMode);

        if (!InPortalMode) return;

        // Tap to switch which portal color you're about to shoot
        if (Input.GetKeyDown(swapColorKey))
            BlueSelected = !BlueSelected;

        // Aim (hold to rotate)
        float aimInput = 0f;
        if (Input.GetKey(aimLeftKey)) aimInput += 1f;   // counter-clockwise
        if (Input.GetKey(aimRightKey)) aimInput -= 1f;  // clockwise
        aimAngle += aimInput * aimSpeed * Time.deltaTime;

        Vector2 dir = AimDirection();

        if (aimPivot != null)
            aimPivot.rotation = Quaternion.Euler(0, 0, aimAngle);

        // Cast every frame so the aim line can show where the shot will land
        RaycastHit2D hit = Physics2D.Raycast(muzzle.position, dir, maxDistance, hitMask);

        if (aimLine != null)
        {
            aimLine.positionCount = 2;
            aimLine.SetPosition(0, muzzle.position);
            Vector3 end = hit.collider != null ? (Vector3)hit.point : muzzle.position + (Vector3)(dir * maxDistance);
            aimLine.SetPosition(1, end);
        }

        if (Input.GetKeyDown(fireKey))
            Fire(hit);
    }

    Vector2 AimDirection()
    {
        float rad = aimAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    void SetPortalMode(bool on)
    {
        InPortalMode = on;

        if (movement != null)
        {
            movement.canMoveHorizontally = !(on && lockHorizontalInPortalMode);
            movement.canJump = !(on && lockJumpInPortalMode);
        }

        if (aimLine != null) aimLine.enabled = on;
    }

    void Fire(RaycastHit2D hit)
    {
        if (hit.collider == null) return; // shot hit nothing

        // Did we hit something that portals are allowed on?
        bool validSurface = (portalSurfaceMask.value & (1 << hit.collider.gameObject.layer)) != 0;
        if (!validSurface) return;

        Transform portal = BlueSelected ? bluePortal : orangePortal;
        PlacePortal(portal, hit.point, hit.normal);
    }

    void PlacePortal(Transform portal, Vector2 point, Vector2 normal)
    {
        portal.position = point + normal * surfaceOffset;
        // Rotate the portal so its "facing" axis points away from the wall
        portal.rotation = Quaternion.FromToRotation(portalFacingAxis, normal);
    }
}