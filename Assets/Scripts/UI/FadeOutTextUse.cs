using UnityEngine;

public class FadeOutTextUse : MonoBehaviour
{
    // �̱��� ���� ����
    public static FadeOutTextUse I;

    [SerializeField] private GameObject fadeOutTextPrefab;
    private Canvas textCanvas;

    private void Awake()
    {
        if ( I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        textCanvas = new GameObject("TextCanvas").AddComponent<Canvas>();
        textCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        textCanvas.sortingOrder = 100;
    }

    public void SpawnFadeOutText(Vector3 position, string message, Color color, bool isUIElement = false)
    {
        if (fadeOutTextPrefab == null || textCanvas == null)
        {
            Debug.LogError("������ �Ǵ� ĵ������ �������� �ʾҽ��ϴ�!");
            return;
        }

        GameObject newTextObject = Instantiate(fadeOutTextPrefab, textCanvas.transform);

        if (isUIElement)
        {
            RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
        }
        else
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
            newTextObject.transform.position = screenPosition;
        }

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
