using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("폭탄 시간")]
    [SerializeField]
    private float countdownTime = 5f;

    [SerializeField]
    private float warningTime = 1.5f;

    [SerializeField]
    private float blinkInterval = 0.1f;


    [Header("폭발")]
    [SerializeField]
    private float explosionRadius = 3f;

    [SerializeField]
    private float explosionForce = 10f;


    private SpriteRenderer spriteRenderer;

    // 이미 카운트다운이 시작됐는지 확인
    private bool isActivated = false;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 처음에는 검은색
        spriteRenderer.color = Color.black;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 이미 작동 중이면 다시 시작하지 않음
        if (isActivated)
            return;

        isActivated = true;

        // 카운트다운 시작
        StartCoroutine(Countdown());
    }


    private IEnumerator Countdown()
    {
        // 5초 중 처음 3.5초 기다림
        yield return new WaitForSeconds(
            countdownTime - warningTime
        );


        // 마지막 1.5초 동안 점멸
        float remainingTime = warningTime;

        while (remainingTime > 0f)
        {
            // 빨간색
            spriteRenderer.color = Color.red;

            yield return new WaitForSeconds(blinkInterval);

            remainingTime -= blinkInterval;


            // 검은색
            spriteRenderer.color = Color.black;

            yield return new WaitForSeconds(blinkInterval);

            remainingTime -= blinkInterval;
        }


        // 시간이 끝나면 폭발
        Explode();
    }


    private void Explode()
    {
        // 폭발 범위 안의 모든 Collider2D 찾기
        Collider2D[] objects =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );


        foreach (Collider2D obj in objects)
        {
            Rigidbody2D rb = obj.attachedRigidbody;

            // Rigidbody2D가 없는 오브젝트는 밀 수 없음
            if (rb == null)
                continue;

            // 자기 자신은 제외
            if (rb.gameObject == gameObject)
                continue;


            // 폭탄 → 오브젝트 방향 계산
            Vector2 direction =
                (rb.worldCenterOfMass - (Vector2)transform.position)
                .normalized;


            // 해당 방향으로 순간적인 힘을 가함
            rb.AddForce(
                direction * explosionForce,
                ForceMode2D.Impulse
            );
        }


        // 폭발 후 폭탄 제거
        Destroy(gameObject);
    }


    // Scene 창에서 폭발 범위를 확인하기 위한 원
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}