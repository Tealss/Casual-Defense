using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public Text blinkingText;
    public GameObject[] setFalseObject;
    public GameObject[] setFadeFalseObject;
    public GameObject[] setTrueObject;

    private bool isBlinking = true;
    private bool hasTriggered = false; // 트리거가 한 번만 발생하도록 관리하는 변수

    void Start()
    {
        StartCoroutine(BlinkText());
    }

    void Update()
    {
        // Input.anyKeyDown이 처음 눌렸을 때만 트리거 실행
        if (Input.anyKeyDown && !hasTriggered)
        {
            hasTriggered = true; // 트리거 실행됨

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
            // CanvasGroup이 없으면 추가
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        float duration = 2f; // 페이드 아웃 시간
        float startAlpha = canvasGroup.alpha;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / duration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        obj.SetActive(false); // 페이드 아웃 후 오브젝트 비활성화
    }
}
