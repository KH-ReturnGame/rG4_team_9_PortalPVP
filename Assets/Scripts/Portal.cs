using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private Portal linkedPortal;

    [SerializeField]
    private float exitOffset = 1f;

    [Header("Exit Angle")]
    [Tooltip("If true, ignore the auto-computed angle between the two portals and use Manual Exit Angle instead. Use this when your sprite's 'forward' doesn't visually line up with the portal's up-vector.")]
    [SerializeField]
    private bool useManualExitAngle = false;

    [Tooltip("Degrees. Only used if Use Manual Exit Angle is on. This becomes both the exit direction (relative to this portal's linked exit point) and the rotation/velocity delta applied to the traveler.")]
    [SerializeField]
    private float manualExitAngleDegrees = 0f;

    // cooldown keyed on the PortalTraveler so the same object can't
    // immediately re-trigger a teleport the moment it exits
    private static HashSet<PortalTraveler> teleportCooldown
        = new HashSet<PortalTraveler>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null)
            return;

        PortalTraveler traveler = rb.GetComponentInParent<PortalTraveler>();
        if (traveler == null)
            return; // not a portal-capable object, ignore

        if (teleportCooldown.Contains(traveler))
            return;

        Teleport(traveler);
    }

    private void Teleport(PortalTraveler traveler)
    {
        teleportCooldown.Add(traveler);

        float angleDifference;
        Vector2 exitDirection;

        if (useManualExitAngle)
        {
            angleDifference = manualExitAngleDegrees;
            exitDirection = Quaternion.Euler(0f, 0f, manualExitAngleDegrees) * linkedPortal.transform.up;
        }
        else
        {
            angleDifference =
                linkedPortal.transform.eulerAngles.z
                - transform.eulerAngles.z
                + 180f;
            exitDirection = linkedPortal.transform.up;
        }

        Vector2 newPosition =
            (Vector2)linkedPortal.transform.position + exitDirection * exitOffset;

        traveler.Teleport(newPosition, angleDifference);

        StartCoroutine(RemoveCooldown(traveler));
    }

    private IEnumerator RemoveCooldown(PortalTraveler traveler)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        teleportCooldown.Remove(traveler);
    }
}