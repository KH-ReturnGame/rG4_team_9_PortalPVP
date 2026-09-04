using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // 연결된 반대편 포탈
    [SerializeField]
    private Portal linkedPortal;

    // 출구 포탈 중앙에서 얼마나 앞쪽으로 나오게 할지
    [SerializeField]
    private float exitOffset = 1f;

    // 방금 순간이동한 Rigidbody2D 목록
    private static HashSet<Rigidbody2D> teleportCooldown
        = new HashSet<Rigidbody2D>();


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 부딪힌 Collider2D에 연결된 Rigidbody2D 가져오기
        Rigidbody2D rb = other.attachedRigidbody;

        // Rigidbody2D가 없다면 포탈 이동시키지 않음
        if (rb == null)
            return;


        // 방금 순간이동한 물체라면 다시 순간이동하지 않음
        if (teleportCooldown.Contains(rb))
            return;


        Teleport(rb);
    }


    private void Teleport(Rigidbody2D rb)
    {
        // 순간이동 중인 물체 목록에 추가
        teleportCooldown.Add(rb);


        // 현재 속도 저장
        Vector2 currentVelocity = rb.linearVelocity;


        // 입구 포탈과 출구 포탈의 Z축 회전 차이 계산
        float angleDifference =
            linkedPortal.transform.eulerAngles.z
            - transform.eulerAngles.z
            + 180f;


        // 속도 벡터를 포탈 각도에 맞게 회전
        Vector2 newVelocity =
            Quaternion.Euler(0f, 0f, angleDifference)
            * currentVelocity;


        // 출구 포탈이 바라보는 방향 구하기
        Vector2 exitDirection = linkedPortal.transform.up;


        // 출구 포탈 중앙보다 약간 앞쪽으로 이동
        rb.position =
            (Vector2)linkedPortal.transform.position
            + exitDirection * exitOffset;


        // 변환된 속도 적용
        rb.linearVelocity = newVelocity;


        // 잠시 뒤 다시 포탈 사용 가능
        StartCoroutine(RemoveCooldown(rb));
    }


    private IEnumerator RemoveCooldown(Rigidbody2D rb)
    {
        // 물리 프레임 두 번 기다림
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        teleportCooldown.Remove(rb);
    }
}
