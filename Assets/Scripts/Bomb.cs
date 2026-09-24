using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Countdown")]

    // 바닥과 처음 접촉했을 때 시작되는 타이머
    [SerializeField]
    private float groundCountdownTime = 12f;

    // 플레이어, 상자 등 다른 오브젝트와
    // 처음 접촉했을 때 시작되는 타이머
    [SerializeField]
    private float objectCountdownTime = 5f;

    // 가장 먼저 끝나는 타이머가
    // 이 시간 이하가 되면 점멸 시작
    [SerializeField]
    private float warningTime = 1.5f;

    // 빨강 ↔ 검정 점멸 간격
    [SerializeField]
    private float blinkInterval = 0.1f;


    [Header("Explosion")]

    // 폭발 범위
    [SerializeField]
    private float explosionRadius = 3f;

    // 폭발로 밀어내는 힘
    [SerializeField]
    private float explosionForce = 10f;


    private SpriteRenderer spriteRenderer;


    // -------------------------
    // 타이머 상태
    // -------------------------

    // 바닥 타이머가 작동 중인지
    private bool groundTimerActive = false;

    // 일반 오브젝트 타이머가 작동 중인지
    private bool objectTimerActive = false;


    // 각 타이머의 남은 시간
    private float groundTimeRemaining;
    private float objectTimeRemaining;


    // 점멸 중인지
    private bool isBlinking = false;

    // 이미 폭발했는지
    private bool hasExploded = false;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 처음에는 검은색
        spriteRenderer.color = Color.black;
    }


    private void Update()
    {
        // 이미 폭발했다면 더 이상 처리하지 않음
        if (hasExploded)
            return;


        // -------------------------
        // 바닥 타이머
        // -------------------------

        if (groundTimerActive)
        {
            groundTimeRemaining -= Time.deltaTime;

            // 바닥 타이머 종료
            if (groundTimeRemaining <= 0f)
            {
                Explode();
                return;
            }
        }


        // -------------------------
        // 오브젝트 타이머
        // -------------------------

        if (objectTimerActive)
        {
            objectTimeRemaining -= Time.deltaTime;

            // 오브젝트 타이머 종료
            if (objectTimeRemaining <= 0f)
            {
                Explode();
                return;
            }
        }


        // 현재 가장 빨리 끝나는 타이머의
        // 남은 시간 확인
        float shortestTime = Mathf.Infinity;

        if (groundTimerActive)
        {
            shortestTime =
                Mathf.Min(
                    shortestTime,
                    groundTimeRemaining
                );
        }

        if (objectTimerActive)
        {
            shortestTime =
                Mathf.Min(
                    shortestTime,
                    objectTimeRemaining
                );
        }


        // 가장 빠른 타이머가 1.5초 이하라면
        // 점멸 시작
        if (shortestTime <= warningTime && !isBlinking)
        {
            isBlinking = true;

            StartCoroutine(Blink());
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 이미 폭발했다면 무시
        if (hasExploded)
            return;


        // =========================
        // 바닥과 충돌
        // =========================

        if (collision.gameObject.CompareTag("Ground"))
        {
            // 바닥 타이머가 아직 시작되지 않았다면
            if (!groundTimerActive)
            {
                groundTimerActive = true;

                groundTimeRemaining =
                    groundCountdownTime;
            }

            return;
        }


        // =========================
        // 그 외의 오브젝트와 충돌
        // =========================

        // 일반 오브젝트 타이머가
        // 아직 시작되지 않았다면 시작
        if (!objectTimerActive)
        {
            objectTimerActive = true;

            objectTimeRemaining =
                objectCountdownTime;
        }
    }


    private IEnumerator Blink()
    {
        while (!hasExploded)
        {
            // 빨간색
            spriteRenderer.color = Color.red;

            yield return new WaitForSeconds(
                blinkInterval
            );


            if (hasExploded)
                yield break;


            // 검은색
            spriteRenderer.color = Color.black;

            yield return new WaitForSeconds(
                blinkInterval
            );
        }
    }


    private void Explode()
    {
        // 중복 폭발 방지
        if (hasExploded)
            return;

        hasExploded = true;


        // 폭발 범위 안의 모든 Collider2D 찾기
        Collider2D[] objects =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );


        foreach (Collider2D obj in objects)
        {
            Rigidbody2D rb =
                obj.attachedRigidbody;


            // Rigidbody2D가 없다면 무시
            if (rb == null)
                continue;


            // 자기 자신은 무시
            if (rb.gameObject == gameObject)
                continue;


            // 폭탄 → 오브젝트 방향 계산
            Vector2 direction =
                rb.worldCenterOfMass
                - (Vector2)transform.position;


            direction.Normalize();


            // 순간적으로 밀어냄
            rb.AddForce(
                direction * explosionForce,
                ForceMode2D.Impulse
            );
        }


        // 폭탄 제거
        Destroy(gameObject);
    }


    // Scene 창에서 폭발 범위 확인
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}