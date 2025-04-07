using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [Tooltip("장애물의 이동 속도")]
    public float moveSpeed = 5f;

    [Tooltip("장애물이 이동할 수 있는 최대 거리 (왼쪽)")]
    public float leftBound = -5f;

    [Tooltip("장애물이 이동할 수 있는 최대 거리 (오른쪽)")]
    public float rightBound = 5f;

    [Tooltip("처음 이동 방향 (true: 오른쪽, false: 왼쪽)")]
    public bool moveRight = true;

    private Vector3 startPosition;

    void Start()
    {
        // 시작 위치 저장
        startPosition = transform.position;
    }

    void Update()
    {
        // 현재 위치 계산
        float currentX = transform.position.x - startPosition.x;

        // 방향 전환 확인
        if (currentX >= rightBound)
        {
            moveRight = false;
        }
        else if (currentX <= leftBound)
        {
            moveRight = true;
        }

        // 이동 방향에 따라 이동
        float direction = moveRight ? 1f : -1f;
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
    }

    // 시각적으로 이동 범위 표시 (에디터에서만 보임)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 leftPoint = transform.position;
        leftPoint.x = transform.position.x + leftBound;

        Vector3 rightPoint = transform.position;
        rightPoint.x = transform.position.x + rightBound;

        Gizmos.DrawSphere(leftPoint, 0.3f);
        Gizmos.DrawSphere(rightPoint, 0.3f);
        Gizmos.DrawLine(leftPoint, rightPoint);
    }
}