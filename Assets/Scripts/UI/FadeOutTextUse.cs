using UnityEngine;
using TMPro;  // TextMeshPro�� ����ϴ� ���, �ʿ� ���ٸ� ����

public class FadeOutTextUse : MonoBehaviour
{
    [SerializeField] private GameObject fadeOutTextPrefab;
    [SerializeField] private Canvas canvas; // �Ϲ� UI ĵ����
    private Canvas textCanvas;             // �ؽ�Ʈ ���� ĵ����

    private void Start()
    {
        // �ؽ�Ʈ ���� ĵ���� ����
        textCanvas = new GameObject("TextCanvas").AddComponent<Canvas>();
        textCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        textCanvas.sortingOrder = 100; // ��ư UI���� ���� ������
    }

    public void SpawnFadeOutText(Vector3 position, string message, Color color, bool isUIElement = false)
    {
        if (fadeOutTextPrefab == null || textCanvas == null)
        {
            Debug.LogError("������ �Ǵ� ĵ������ �������� �ʾҽ��ϴ�!");
            return;
        }

        GameObject newTextObject = Instantiate(fadeOutTextPrefab, textCanvas.transform);

        // UI ����� ���, RectTransform�� ����Ͽ� ��Ȯ�� UI ��ǥ�� ��ġ
        if (isUIElement)
        {
            RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;  // ĵ���� ������ UI ��ǥ�� ����

        }
        else
        {
            // 3D ���� ��ǥ���� ȭ�� ��ǥ�� ��ȯ
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
            newTextObject.transform.position = screenPosition;
        }

        // �ؽ�Ʈ�� �����ϰ� ���̵� �ƿ� �ִϸ��̼� ����
        FadeOutText fadeOutText = newTextObject.GetComponent<FadeOutText>();
        if (fadeOutText != null)
        {
            fadeOutText.Initialize(message, color);
        }
        else
        {
            Debug.LogError("FadeOutText ������Ʈ�� ã�� �� �����ϴ�!");
        }
    }
}
