using UnityEngine;

public class VerticalObstacleMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [Tooltip("장애물의 이동 속도")]
    public float moveSpeed = 3f;

    [Tooltip("장애물이 이동할 수 있는 최대 거리 (아래)")]
    public float bottomBound = -3f;

    [Tooltip("장애물이 이동할 수 있는 최대 거리 (위)")]
    public float topBound = 3f;

    [Tooltip("처음 이동 방향 (true: 위, false: 아래)")]
    public bool moveUp = true;

    private Vector3 startPosition;

    void Start()
    {
        // 시작 위치 저장
        startPosition = transform.position;
    }

    void Update()
    {
        // 현재 위치 계산
        float currentY = transform.position.y - startPosition.y;

        // 방향 전환 확인
        if (currentY >= topBound)
        {
            moveUp = false;
        }
        else if (currentY <= bottomBound)
        {
            moveUp = true;
        }

        // 이동 방향에 따라 이동
        float direction = moveUp ? 1f : -1f;
        transform.Translate(Vector3.up * direction * moveSpeed * Time.deltaTime);
    }

    // 시각적으로 이동 범위 표시 (에디터에서만 보임)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 bottomPoint = transform.position;
        bottomPoint.y = transform.position.y + bottomBound;

        Vector3 topPoint = transform.position;
        topPoint.y = transform.position.y + topBound;

        Gizmos.DrawSphere(bottomPoint, 0.3f);
        Gizmos.DrawSphere(topPoint, 0.3f);
        Gizmos.DrawLine(bottomPoint, topPoint);
    }
}