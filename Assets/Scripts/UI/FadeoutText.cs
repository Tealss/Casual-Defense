using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOutText : MonoBehaviour
{
    [Header("셋팅")]
    public float fadeDuration = 2f; // 페이드 아웃에 걸리는 시간 (초)
    public float moveSpeed = 50f; // 텍스트가 이동할 속도 (위로 이동하는 속도)

    private Text textComponent;
    private Vector3 initialPosition;
    private Color initialColor;

    void Start()
    {
        textComponent = GetComponent<Text>();
        initialPosition = transform.position;
        initialColor = textComponent.color;

        // 페이드 아웃 코루틴 시작
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // 텍스트의 alpha 값을 점차 감소시키며 이동
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            textComponent.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            // 텍스트를 위로 이동
            transform.position = initialPosition + Vector3.up * moveSpeed * (elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 마지막에는 완전히 사라지게 하고 텍스트를 비활성화
        textComponent.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        gameObject.SetActive(false); // 텍스트를 비활성화
    }
}