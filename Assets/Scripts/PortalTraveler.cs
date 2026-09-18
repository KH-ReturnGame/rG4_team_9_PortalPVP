using UnityEngine;

public class PortalTraveler : MonoBehaviour
{
    [Tooltip("포탈을 나올 때 오브젝트 자체도 출구 방향으로 회전할지 여부")]
    public bool rotateVisualOrientation = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // newPosition = 순간이동할 위치
    // exitDirection = 출구 포탈이 바라보는 방향
    public void Teleport(Vector2 newPosition, Vector2 exitDirection)
    {
        // 포탈에 들어가기 직전의 '속력'만 저장
        float speed = rb.linearVelocity.magnitude;


        // 반대편 포탈 중앙으로 이동
        rb.position = newPosition;


        // 출구 방향으로 오브젝트 자체도 회전시키고 싶다면
        if (rotateVisualOrientation)
        {
            float angle =
                Mathf.Atan2(exitDirection.y, exitDirection.x)
                * Mathf.Rad2Deg
                - 90f;

            rb.rotation = angle;
        }


        // 기존 방향은 버리고,
        // 출구 포탈 방향으로 같은 속력으로 발사
        rb.linearVelocity =
            exitDirection.normalized * speed;
    }
}