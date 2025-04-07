using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("맵 설정")]
    [SerializeField] private GameObject[] mapPrefabs; // 3개의 맵 프리팹 배열
    [SerializeField] private int mapSize = 50; // 각 맵 세그먼트의 크기
    [SerializeField] private int initialMapCount = 3; // 초기에 생성할 맵 세그먼트 수
    [SerializeField] private int maxMapSegments = 10; // 최대로 유지할 맵 세그먼트 수
    [SerializeField] private float playerLookAheadDistance = 100f; // 플레이어 앞에 맵을 생성할 거리
    [SerializeField] private bool useSequentialPattern = true; // 순차적으로 맵 프리팹을 사용할지 여부

    [Header("참조")]
    [SerializeField] private Transform playerTransform; // 플레이어의 Transform 참조

    private List<GameObject> activeMapSegments = new List<GameObject>(); // 활성화된 맵 세그먼트 추적 리스트
    private float furthestMapZ = 0f; // 가장 멀리 생성된 맵의 Z 위치
    private int currentPrefabIndex = 0; // 현재 사용할 프리팹 인덱스

    private void Start()
    {
        // 맵 프리팹 확인
        if (mapPrefabs == null || mapPrefabs.Length == 0)
        {
            Debug.LogError("맵 프리팹이 할당되지 않았습니다!");
            return;
        }

        // 초기 맵 세그먼트 생성
        for (int i = 0; i < initialMapCount; i++)
        {
            CreateMapSegment();
        }

        // 플레이어가 설정되지 않았다면 찾기
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerController>()?.transform;
            if (playerTransform == null)
            {
                Debug.LogWarning("플레이어 Transform을 찾을 수 없습니다. 메인 카메라를 참조로 사용합니다.");
                playerTransform = Camera.main.transform;
            }
        }
    }

    private void Update()
    {
        // 플레이어 앞에 더 많은 맵 세그먼트가 필요한지 확인
        if (playerTransform != null && furthestMapZ - playerTransform.position.z < playerLookAheadDistance)
        {
            CreateMapSegment();
            RemoveDistantMapSegments();
        }
    }

    private void CreateMapSegment()
    {
        if (mapPrefabs == null || mapPrefabs.Length == 0)
        {
            Debug.LogError("맵 프리팹이 할당되지 않았습니다!");
            return;
        }

        // 현재 사용할 맵 프리팹 선택
        GameObject prefabToUse;

        if (useSequentialPattern)
        {
            // 순차적으로 맵 프리팹 사용 (0, 1, 2, 0, 1, 2, ...)
            prefabToUse = mapPrefabs[currentPrefabIndex];

            // 다음 인덱스로 이동 (순환식)
            currentPrefabIndex = (currentPrefabIndex + 1) % mapPrefabs.Length;
        }
        else
        {
            // 랜덤하게 맵 프리팹 선택
            prefabToUse = mapPrefabs[Random.Range(0, mapPrefabs.Length)];
        }

        // 새 맵 세그먼트의 위치 계산
        Vector3 position = new Vector3(0, 0, furthestMapZ);

        // 맵 프리팹 인스턴스화
        GameObject newMap = Instantiate(prefabToUse, position, Quaternion.identity);
        newMap.transform.parent = transform; // 정리를 위해 이 오브젝트의 자식으로 설정
        newMap.name = "맵세그먼트_" + activeMapSegments.Count + "_" + currentPrefabIndex;

        // 활성 세그먼트 리스트에 추가
        activeMapSegments.Add(newMap);

        // 가장 먼 Z 위치 업데이트
        furthestMapZ += mapSize;
    }

    private void RemoveDistantMapSegments()
    {
        // 최대 개수보다 많은 세그먼트가 있다면 가장 오래된 것을 제거
        if (activeMapSegments.Count > maxMapSegments)
        {
            // 가장 오래된 맵 세그먼트 가져오기
            GameObject oldestSegment = activeMapSegments[0];

            // 리스트에서 제거
            activeMapSegments.RemoveAt(0);

            // 게임 오브젝트 파괴
            Destroy(oldestSegment);
        }
    }

#if UNITY_EDITOR
    // 에디터에서 맵 크기를 시각화하는 헬퍼 메서드
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < 5; i++)
        {
            Vector3 pos = new Vector3(0, 0, i * mapSize);
            Gizmos.DrawWireCube(pos + new Vector3(0, 0, mapSize / 2), new Vector3(mapSize, 1, mapSize));
        }
    }
#endif
}