using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private Portal linkedPortal;

    [SerializeField]
    private float exitOffset = 1f;

    // cooldown keyed on the ROOT PortalTraveler, not individual limb
    // rigidbodies, so multiple limbs touching the same portal frame
    // don't trigger multiple redundant teleports
    private static HashSet<PortalTraveler> teleportCooldown
        = new HashSet<PortalTraveler>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null)
            return;

        // whichever limb touched the trigger, find its rig's root
        PortalTraveler traveler = rb.GetComponentInParent<PortalTraveler>();
        if (traveler == null)
            return; // not a portal-capable object, ignore

        if (teleportCooldown.Contains(traveler))
            return;

        Teleport(traveler, rb);
    }

    private void Teleport(PortalTraveler traveler, Rigidbody2D triggeringBody)
    {
        teleportCooldown.Add(traveler);

        float angleDifference =
            linkedPortal.transform.eulerAngles.z
            - transform.eulerAngles.z
            + 180f;

        Vector2 exitDirection = linkedPortal.transform.up;
        Vector2 newRootPosition =
            (Vector2)linkedPortal.transform.position + exitDirection * exitOffset;

        traveler.TeleportGroup(newRootPosition, angleDifference);

        StartCoroutine(RemoveCooldown(traveler));
    }

    private IEnumerator RemoveCooldown(PortalTraveler traveler)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        teleportCooldown.Remove(traveler);
    }
}
