using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPUIManager : MonoBehaviour
{
    // 싱글턴 인스턴스
    private static HPUIManager _instance;

    // 싱글턴 프로퍼티
    public static HPUIManager Instance
    {
        get
        {
            // 인스턴스가 없으면 찾아보기
            if (_instance == null)
            {
                _instance = FindObjectOfType<HPUIManager>();

                // 씬에 없으면 새로 생성
                if (_instance == null)
                {
                    GameObject obj = new GameObject("HPUIManager");
                    _instance = obj.AddComponent<HPUIManager>();
                }
            }
            return _instance;
        }
    }

    // HP UI 컴포넌트
    [Header("HP UI 컴포넌트")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    // HP 관련 변수
    [Header("HP 설정")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    // 애니메이션 관련 변수
    [Header("슬라이더 애니메이션")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private float animationSpeed = 5f;
    private float targetHPValue;

    // 색상 변화 설정
    [Header("HP 색상 설정")]
    [SerializeField] private bool useColorChange = true;
    [SerializeField] private Color fullHPColor = Color.green;
    [SerializeField] private Color mediumHPColor = Color.yellow;
    [SerializeField] private Color lowHPColor = Color.red;
    [SerializeField] private float mediumHPThreshold = 0.5f; // 50%
    [SerializeField] private float lowHPThreshold = 0.2f;    // 20%

    private void Awake()
    {
        // 싱글턴 구현
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // 초기값 설정
        currentHP = maxHP;
        targetHPValue = 1f;

        // 초기 UI 설정
        UpdateUI(false);
    }

    private void Start()
    {
        // 컴포넌트가 없을 경우 자동으로 찾기 시도
        if (hpSlider == null)
        {
            hpSlider = FindObjectOfType<Slider>();
            Debug.LogWarning("HP Slider가 할당되지 않아 자동으로 찾았습니다. 인스펙터에서 직접 할당하는 것을 권장합니다.");
        }

        if (hpText == null)
        {
            hpText = FindObjectOfType<TextMeshProUGUI>();
            Debug.LogWarning("HP Text가 할당되지 않아 자동으로 찾았습니다. 인스펙터에서 직접 할당하는 것을 권장합니다.");
        }
    }

    private void Update()
    {
        // 애니메이션 처리
        if (useAnimation && hpSlider != null)
        {
            // 부드러운 HP 감소/증가 애니메이션
            float currentValue = hpSlider.value;
            float newValue = Mathf.Lerp(currentValue, targetHPValue, Time.deltaTime * animationSpeed);
            hpSlider.value = newValue;
        }
    }

    /// <summary>
    /// HP 값을 설정
    /// </summary>
    /// <param name="newHP">새로운 HP 값</param>
    public void SetHP(float newHP)
    {
        currentHP = Mathf.Clamp(newHP, 0f, maxHP);
        targetHPValue = currentHP / maxHP;

        UpdateUI(useAnimation);
    }

    /// <summary>
    /// HP를 일정량 감소
    /// </summary>
    /// <param name="damage">데미지 양</param>
    public void TakeDamage(float damage)
    {
        if (damage < 0) return; // 음수 데미지 방지

        currentHP = Mathf.Clamp(currentHP - damage, 0f, maxHP);
        targetHPValue = currentHP / maxHP;

        UpdateUI(useAnimation);

        // HP가 0이 되면 이벤트 발생
        if (currentHP <= 0)
        {
            OnPlayerDeath();
        }
    }

    /// <summary>
    /// HP를 일정량 회복
    /// </summary>
    /// <param name="healAmount">회복량</param>
    public void Heal(float healAmount)
    {
        if (healAmount < 0) return; // 음수 회복량 방지

        currentHP = Mathf.Clamp(currentHP + healAmount, 0f, maxHP);
        targetHPValue = currentHP / maxHP;

        UpdateUI(useAnimation);
    }

    /// <summary>
    /// UI 업데이트
    /// </summary>
    /// <param name="animated">애니메이션 사용 여부</param>
    private void UpdateUI(bool animated)
    {
        if (hpSlider != null)
        {
            if (!animated)
            {
                hpSlider.value = currentHP / maxHP;
            }

            // 색상 변경 처리
            UpdateSliderColor();
        }

        if (hpText != null)
        {
            hpText.text = $"{Mathf.Ceil(currentHP)} / {maxHP}";
        }
    }

    /// <summary>
    /// 슬라이더 색상 업데이트
    /// </summary>
    private void UpdateSliderColor()
    {
        if (!useColorChange || hpSlider == null) return;

        // 슬라이더의 Fill 이미지 가져오기
        Image fillImage = hpSlider.fillRect.GetComponent<Image>();
        if (fillImage == null) return;

        float hpRatio = currentHP / maxHP;

        // HP 비율에 따라 색상 변경
        if (hpRatio <= lowHPThreshold)
        {
            fillImage.color = lowHPColor;
        }
        else if (hpRatio <= mediumHPThreshold)
        {
            fillImage.color = mediumHPColor;
        }
        else
        {
            fillImage.color = fullHPColor;
        }
    }

    /// <summary>
    /// 최대 HP 설정
    /// </summary>
    /// <param name="newMaxHP">새로운 최대 HP</param>
    public void SetMaxHP(float newMaxHP)
    {
        if (newMaxHP <= 0) return; // 0 이하의 최대 HP 방지

        maxHP = newMaxHP;

        // 현재 HP 비율 유지하며 조정
        float hpRatio = currentHP / maxHP;
        currentHP = maxHP * hpRatio;

        UpdateUI(false);
    }

    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    private void OnPlayerDeath()
    {
        Debug.Log("플레이어 HP가 0이 되었습니다!");
        // 여기에 사망 관련 이벤트/로직 추가
    }

    /// <summary>
    /// 현재 HP 비율 반환 (0.0 ~ 1.0)
    /// </summary>
    public float GetHPRatio()
    {
        return currentHP / maxHP;
    }

    /// <summary>
    /// 현재 HP 값 반환
    /// </summary>
    public float GetCurrentHP()
    {
        return currentHP;
    }

    /// <summary>
    /// 최대 HP 값 반환
    /// </summary>
    public float GetMaxHP()
    {
        return maxHP;
    }
}