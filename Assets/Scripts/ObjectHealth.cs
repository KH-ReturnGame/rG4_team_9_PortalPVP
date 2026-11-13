using System;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    private float maxHealth = 100f;

    private float currentHealth;

    [Header("Death")]
    [SerializeField]
    private bool destroyOnDeath = true;

    private bool isDead = false;

    // HP가 0이 되었을 때 발생하는 이벤트
    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f)
            return;

        currentHealth = Mathf.Max(
            currentHealth - damage,
            0f
        );

        Debug.Log($"{gameObject.name} HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        currentHealth = Mathf.Min(
            currentHealth + amount,
            maxHealth
        );
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // 다른 스크립트에 사망 알림
        OnDeath?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}