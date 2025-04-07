using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용 시 필요

public class ScoreUIManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ScoreUIManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI scoreText; // TextMeshPro 사용 시
    // [SerializeField] private Text scoreText; // 기본 UI Text 사용 시
    
    [SerializeField] private string scorePrefix = "SCORE: "; // 점수 앞에 표시할 텍스트
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // 다른 씬으로 전환해도 유지되게 하려면 아래 라인 추가
        // DontDestroyOnLoad(gameObject);
        
        // 초기화 시 스코어 텍스트 컴포넌트가 없으면 경고
        if (scoreText == null)
        {
            Debug.LogWarning("ScoreUIManager: scoreText가 할당되지 않았습니다.");
        }
    }
    
    // 점수 설정 및 UI 업데이트
    public void SetScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = scorePrefix + score.ToString();
        }
    }
}