using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 5f;  // 앞으로 달리는 속도
    [SerializeField] private float horizontalSpeed = 4f;  // 좌우 이동 속도
    [SerializeField] private float jumpForce = 8f;  // 점프 힘

    [Header("Ground Check")]
    [SerializeField] private float rayDistance = 1.1f;  // 레이캐스트 거리 (캐릭터 높이의 절반 + 여유값)
    [SerializeField] private LayerMask groundLayer;  // 바닥 레이어

    [Header("Game Over Settings")]
    [SerializeField] private float gameOverHeight = -5f;  // 게임 오버가 되는 Y 위치

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;  // 최대 체력
    [SerializeField] private LayerMask obstacleLayer;  // 장애물 레이어
    [SerializeField] private float collisionCooldown = 0.5f;  // 충돌 쿨다운 시간(초)

    [Header("Animation Settings")]
    [SerializeField] private string jumpAnimationName = "Jump";  // 점프 애니메이션 이름
    
    [Header("Coin Settings")]
    [SerializeField] private LayerMask coinLayer;  // 코인 레이어
    [SerializeField] private int coinValue = 100;  // 코인 획득 시 증가하는 점수

    private Rigidbody rb;
    private Animator animator;  // Animator 컴포넌트 참조 추가
    private bool isGrounded;
    private float horizontalInput;
    private bool isGameOver = false;
    private int currentHealth;
    private float lastCollisionTime = 0f;  // 마지막 충돌 시간
    private int currentScore = 0;  // 현재 점수

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();  // Animator 컴포넌트 가져오기
        currentHealth = maxHealth;
        currentScore = 0;  // 점수 초기화

        HPUIManager.Instance.SetHP(currentHealth);
        
        // ScoreUIManager가 있다면 초기 점수 설정
        if (ScoreUIManager.Instance != null)
        {
            ScoreUIManager.Instance.SetScore(currentScore);
        }
        else
        {
            Debug.LogWarning("ScoreUIManager가 없습니다. UI에 점수가 표시되지 않을 수 있습니다.");
        }
    }

    private void Update()
    {
        // 게임 오버 상태에서 R 키를 누르면 씬 재시작
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
            return;
        }

        // 게임 오버 상태라면 다른 입력 처리 안함
        if (isGameOver) return;

        // 게임 오버 체크
        CheckGameOver();

        // 입력 처리 (구버전 Input 시스템 사용)
        horizontalInput = Input.GetAxis("Horizontal");

        // 점프 입력 처리 (스페이스바)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // 지면 체크
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        // 게임 오버 상태라면 움직임 처리 안함
        if (isGameOver) return;

        // 자동으로 앞으로 이동
        MoveForward();
        // 좌우 이동
        MoveHorizontal();
    }

    private void MoveForward()
    {
        // 현재 속도 가져오기
        Vector3 velocity = rb.linearVelocity;
        // z축 속도만 forwardSpeed로 고정
        velocity.z = forwardSpeed;
        // 새로운 속도 적용
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, velocity.z);
    }

    private void MoveHorizontal()
    {
        // 좌우 이동 (x축 속도 조절)
        Vector3 velocity = rb.linearVelocity;
        velocity.x = horizontalInput * horizontalSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
    }

    private void Jump()
    {
        // 수직 방향으로 힘 가하기
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        // "Jump" 애니메이션 직접 재생 (Trigger 등 사용하지 않음)
        animator.Play(jumpAnimationName, 0, 0f);
    }

    private void CheckGrounded()
    {
        // 플레이어 본체에서 직접 아래로 레이캐스트 사용하여 지면 체크
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance, groundLayer);
        // 디버그용 레이 표시
        Debug.DrawRay(transform.position, Vector3.down * rayDistance, isGrounded ? Color.green : Color.red);
    }

    private void CheckGameOver()
    {
        // 플레이어의 Y 위치가 gameOverHeight 이하면 게임 오버
        if (transform.position.y <= gameOverHeight && !isGameOver)
        {
            GameOver();
        }

        // 체력이 0이 되면 게임 오버
        if (currentHealth <= 0 && !isGameOver)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        // 게임 오버 상태로 설정
        isGameOver = true;

        // 콘솔에 게임 오버 메시지 출력
        Debug.Log("게임 오버");

        // 체력이 0이 되었을 때의 메시지
        if (currentHealth <= 0)
        {
            Debug.Log("체력이 모두 소진되었습니다");
        }

        // 게임 일시 정지
        Time.timeScale = 0f;

        // 재시작 안내 메시지 출력
        Debug.Log("R 키를 눌러 재시작하세요");
    }

    private void RestartScene()
    {
        // 시간 스케일 복구
        Time.timeScale = 1f;

        // 현재 활성화된 씬 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void DecreaseHealth()
    {
        // 체력 감소
        currentHealth--;

        HPUIManager.Instance.SetHP(currentHealth);

        // 체력이 0이 되면 게임 오버 상태 확인
        if (currentHealth <= 0)
        {
            CheckGameOver();
        }
    }
    
    // 점수 증가 메서드
    private void IncreaseScore(int amount)
    {
        // 점수 증가
        currentScore += amount;
        
        // UI 업데이트
        if (ScoreUIManager.Instance != null)
        {
            ScoreUIManager.Instance.SetScore(currentScore);
        }
        
        // 디버그 메시지
        Debug.Log("코인 획득! 현재 점수: " + currentScore);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 쿨다운 시간 체크 - 마지막 충돌 이후 일정 시간이 지나지 않았으면 무시
        if (Time.time - lastCollisionTime < collisionCooldown)
        {
            return;
        }

        // 장애물 레이어와 충돌했는지 확인
        if (((1 << collision.gameObject.layer) & obstacleLayer.value) != 0)
        {
            // 충돌 시간 기록
            lastCollisionTime = Time.time;

            // 체력 감소
            DecreaseHealth();

            // 장애물 제거
            Destroy(collision.gameObject);

            // 디버그 메시지
            Debug.Log("장애물과 충돌: 남은 체력 " + currentHealth + " (충돌 시간: " + Time.time + ")");
        }
        
        // 코인 레이어와 충돌했는지 확인
        if (((1 << collision.gameObject.layer) & coinLayer.value) != 0)
        {
            // 점수 증가
            IncreaseScore(coinValue);
            
            // 코인 제거
            Destroy(collision.gameObject);
            
            // 디버그 메시지
            Debug.Log("코인 획득! 총 점수: " + currentScore);
        }
    }

    // 디버깅을 위한 시각화
    private void OnDrawGizmosSelected()
    {
        // 지면 체크 레이 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);

        // 게임 오버 높이 시각화
        Gizmos.color = Color.red;
        Vector3 gameOverLineStart = new Vector3(-10f, gameOverHeight, 0f);
        Vector3 gameOverLineEnd = new Vector3(10f, gameOverHeight, 0f);
        Gizmos.DrawLine(gameOverLineStart, gameOverLineEnd);
    }
}