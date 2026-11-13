using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectHealth))]
public class Bomb : MonoBehaviour
{
    [Header("Countdown")]
    [SerializeField] private float groundCountdownTime = 12f;
    [SerializeField] private float objectCountdownTime = 5f;
    [SerializeField] private float warningTime = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float maxExplosionDamage = 100f;

    private SpriteRenderer spriteRenderer;
    private ObjectHealth health;

    private bool groundTimerActive = false;
    private bool objectTimerActive = false;

    private float groundTimeRemaining;
    private float objectTimeRemaining;

    private bool isBlinking = false;
    private bool hasExploded = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<ObjectHealth>();

        spriteRenderer.color = Color.black;
    }

    private void OnEnable()
    {
        // 폭탄 HP가 0이 되면 Explode 실행
        health.OnDeath += Explode;
    }

    private void OnDisable()
    {
        health.OnDeath -= Explode;
    }

    private void Update()
    {
        if (hasExploded)
            return;

        // 바닥 타이머
        if (groundTimerActive)
        {
            groundTimeRemaining -= Time.deltaTime;

            if (groundTimeRemaining <= 0f)
            {
                TimerFinished();
                return;
            }
        }

        // 오브젝트 타이머
        if (objectTimerActive)
        {
            objectTimeRemaining -= Time.deltaTime;

            if (objectTimeRemaining <= 0f)
            {
                TimerFinished();
                return;
            }
        }

        // 두 타이머 중 더 빨리 끝나는 시간
        float shortestTime = Mathf.Infinity;

        if (groundTimerActive)
        {
            shortestTime = Mathf.Min(
                shortestTime,
                groundTimeRemaining
            );
        }

        if (objectTimerActive)
        {
            shortestTime = Mathf.Min(
                shortestTime,
                objectTimeRemaining
            );
        }

        // 폭발 직전 점멸
        if (shortestTime <= warningTime && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(Blink());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded)
            return;

        // 바닥 충돌 → 12초
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!groundTimerActive)
            {
                groundTimerActive = true;
                groundTimeRemaining = groundCountdownTime;
            }

            return;
        }

        // 다른 오브젝트 충돌 → 5초
        if (!objectTimerActive)
        {
            objectTimerActive = true;
            objectTimeRemaining = objectCountdownTime;
        }
    }

    // 타이머 종료 시 폭탄 자신에게 데미지
    private void TimerFinished()
    {
        // 현재 체력만큼 피해를 줘서 HP를 0으로 만듦
        health.TakeDamage(health.GetCurrentHealth());
    }

    private IEnumerator Blink()
    {
        while (!hasExploded)
        {
            spriteRenderer.color = Color.red;

            yield return new WaitForSeconds(blinkInterval);

            if (hasExploded)
                yield break;

            spriteRenderer.color = Color.black;

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    // HP가 0이 되었을 때 호출되는 함수
    private void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        Vector2 explosionCenter = transform.position;

        Collider2D[] objects =
            Physics2D.OverlapCircleAll(
                explosionCenter,
                explosionRadius
            );

        HashSet<ObjectHealth> damagedObjects =
            new HashSet<ObjectHealth>();

        HashSet<Rigidbody2D> pushedObjects =
            new HashSet<Rigidbody2D>();

        foreach (Collider2D obj in objects)
        {
            // 폭탄 자신의 HP는 다시 감소시키지 않음
            if (obj.GetComponentInParent<Bomb>() == this)
                continue;

            Vector2 closestPoint =
                obj.ClosestPoint(explosionCenter);

            float distance = Vector2.Distance(
                explosionCenter,
                closestPoint
            );

            float damageRatio = Mathf.Clamp01(
                1f - distance / explosionRadius
            );

            float damage =
                maxExplosionDamage * damageRatio;

            // 다른 오브젝트에 데미지
            ObjectHealth targetHealth =
                obj.GetComponentInParent<ObjectHealth>();

            if (targetHealth != null &&
                damagedObjects.Add(targetHealth))
            {
                targetHealth.TakeDamage(damage);
            }

            // 폭발력
            Rigidbody2D rb = obj.attachedRigidbody;

            if (rb == null)
                continue;

            if (!pushedObjects.Add(rb))
                continue;

            Vector2 direction =
                rb.worldCenterOfMass - explosionCenter;

            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector2.up;

            direction.Normalize();

            rb.AddForce(
                direction * explosionForce,
                ForceMode2D.Impulse
            );
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}