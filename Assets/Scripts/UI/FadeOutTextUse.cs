using UnityEngine;
using TMPro;  // TextMeshPro를 사용하는 경우, 필요 없다면 제거

public class FadeOutTextUse : MonoBehaviour
{
    [SerializeField] private GameObject fadeOutTextPrefab;
    [SerializeField] private Canvas canvas; // 일반 UI 캔버스
    private Canvas textCanvas;             // 텍스트 전용 캔버스

    private void Start()
    {
        // 텍스트 전용 캔버스 생성
        textCanvas = new GameObject("TextCanvas").AddComponent<Canvas>();
        textCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        textCanvas.sortingOrder = 100; // 버튼 UI보다 위로 렌더링
    }

    public void SpawnFadeOutText(Vector3 position, string message, Color color, bool isUIElement = false)
    {
        if (fadeOutTextPrefab == null || textCanvas == null)
        {
            Debug.LogError("프리팹 또는 캔버스가 설정되지 않았습니다!");
            return;
        }

        GameObject newTextObject = Instantiate(fadeOutTextPrefab, textCanvas.transform);

        // UI 요소일 경우, RectTransform을 사용하여 정확한 UI 좌표에 배치
        if (isUIElement)
        {
            RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;  // 캔버스 내에서 UI 좌표로 설정

        }
        else
        {
            // 3D 월드 좌표에서 화면 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
            newTextObject.transform.position = screenPosition;
        }

        // 텍스트를 설정하고 페이드 아웃 애니메이션 실행
        FadeOutText fadeOutText = newTextObject.GetComponent<FadeOutText>();
        if (fadeOutText != null)
        {
            fadeOutText.Initialize(message, color);
        }
        else
        {
            Debug.LogError("FadeOutText 컴포넌트를 찾을 수 없습니다!");
        }
    }
}
