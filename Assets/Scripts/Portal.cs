using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // 이 포탈과 연결된 반대편 포탈
    [SerializeField]
    private Portal linkedPortal;

    // 출구 포탈의 중앙에서 얼마나 앞쪽에 나오게 할지
    [SerializeField]
    private float exitOffset = 1f;


    // 방금 순간이동한 오브젝트를 저장
    // 순간이동 직후 반대편 포탈에 다시 들어가는 것을 방지
    private static HashSet<PortalTraveler> teleportCooldown
        = new HashSet<PortalTraveler>();


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 포탈에 들어온 오브젝트의 Rigidbody2D를 가져옴
        Rigidbody2D rb = other.attachedRigidbody;

        // Rigidbody2D가 없다면 무시
        if (rb == null)
            return;


        // 해당 오브젝트에서 PortalTraveler를 찾음
        PortalTraveler traveler =
            rb.GetComponentInParent<PortalTraveler>();

        // PortalTraveler가 없다면
        // 포탈을 사용할 수 없는 오브젝트이므로 무시
        if (traveler == null)
            return;


        // 방금 순간이동한 오브젝트라면 무시
        if (teleportCooldown.Contains(traveler))
            return;


        // 순간이동 실행
        Teleport(traveler);
    }


    private void Teleport(PortalTraveler traveler)
    {
        // 순간이동 중인 오브젝트로 등록
        teleportCooldown.Add(traveler);


        // 출구 포탈이 바라보는 방향
        // 현재는 포탈의 로컬 X축(right)을 앞쪽으로 사용
        Vector2 exitDirection =
            linkedPortal.transform.right;


        // 출구 포탈 중앙에서
        // exitOffset만큼 앞쪽의 위치 계산
        Vector2 newPosition =
            (Vector2)linkedPortal.transform.position
            + exitDirection * exitOffset;


        // PortalTraveler에게
        // "이 위치로 이동하고 이 방향으로 나가라"고 전달
        traveler.Teleport(
            newPosition,
            exitDirection
        );


        // 잠시 후 다시 포탈을 사용할 수 있도록 함
        StartCoroutine(RemoveCooldown(traveler));
    }


    private IEnumerator RemoveCooldown(PortalTraveler traveler)
    {
        // 물리 프레임 두 번 기다림
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        // 다시 포탈 사용 가능
        teleportCooldown.Remove(traveler);
    }
}