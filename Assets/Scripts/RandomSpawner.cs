using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [Header("What to spawn")]
    [SerializeField] private GameObject prefab;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 2f;
    [Tooltip("If enabled, the wait time is randomized between min and max instead.")]
    [SerializeField] private bool randomizeInterval = false;
    [SerializeField] private float minInterval = 1f;
    [SerializeField] private float maxInterval = 4f;

    [Header("Spawn area (centered on this object)")]
    [SerializeField] private Vector2 areaSize = new Vector2(10f, 6f);

    [Header("Limits")]
    [Tooltip("0 = unlimited")]
    [SerializeField] private int maxSpawned = 0;
    [SerializeField] private bool parentToSpawner = true;

    private float timer;
    private int spawnedCount;

    private void Start()
    {
        timer = GetNextInterval();
    }

    private void Update()
    {
        if (prefab == null) return;
        if (maxSpawned > 0 && spawnedCount >= maxSpawned) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawn();
            timer = GetNextInterval();
        }
    }

    private void Spawn()
    {
        Vector2 center = transform.position;
        Vector2 half = areaSize * 0.5f;

        Vector2 pos = new Vector2(
            Random.Range(center.x - half.x, center.x + half.x),
            Random.Range(center.y - half.y, center.y + half.y)
        );

        Transform parent = parentToSpawner ? transform : null;
        Instantiate(prefab, pos, Quaternion.identity, parent);
        spawnedCount++;
    }

    private float GetNextInterval()
    {
        return randomizeInterval ? Random.Range(minInterval, maxInterval) : spawnInterval;
    }

    // Draws the spawn box in the Scene view (always visible, brighter when selected)
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0.4f, 0.9f);
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}