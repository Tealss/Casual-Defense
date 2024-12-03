using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public Text blinkingText;
    public GameObject[] setFalseObject;
    public GameObject[] setFadeFalseObject;
    public GameObject[] setTrueObject;

    private bool isBlinking = true;
    private bool hasTriggered = false; // Ʈ���Ű� �� ���� �߻��ϵ��� �����ϴ� ����

    void Start()
    {
        StartCoroutine(BlinkText());
    }

    void Update()
    {
        // Input.anyKeyDown�� ó�� ������ ���� Ʈ���� ����
        if (Input.anyKeyDown && !hasTriggered)
        {
            hasTriggered = true; // Ʈ���� �����

            if (setFalseObject != null)
            {
                for (int i = 0; i < setFalseObject.Length; i++)
                {
                    setFalseObject[i].SetActive(false);
                }

                for (int i = 0; i < setTrueObject.Length; i++)
                {
                    setTrueObject[i].SetActive(true);
                }

                if (setFadeFalseObject != null)
                {
                    for (int i = 0; i < setFadeFalseObject.Length; i++)
                    {
                        StartCoroutine(FadeOutAndDisable(setFadeFalseObject[i]));
                    }
                }
            }

            isBlinking = false;
            blinkingText.enabled = true;
        }
    }

    private System.Collections.IEnumerator BlinkText()
    {
        while (isBlinking)
        {
            blinkingText.enabled = !blinkingText.enabled;
            yield return new WaitForSeconds(2f);
        }
    }

    private System.Collections.IEnumerator FadeOutAndDisable(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // CanvasGroup�� ������ �߰�
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        float duration = 2f; // ���̵� �ƿ� �ð�
        float startAlpha = canvasGroup.alpha;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / duration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        obj.SetActive(false); // ���̵� �ƿ� �� ������Ʈ ��Ȱ��ȭ
    }
}
