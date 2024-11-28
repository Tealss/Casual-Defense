using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutEffect : MonoBehaviour
{
    private Image image; // 페이드 아웃 효과를 적용할 이미지
    private float fadeDuration = 0.2f; // 페이드 인/아웃 효과의 지속 시간
    private float randomTime; // 랜덤 시간

    void Start()
    {
        // 이미지 컴포넌트를 자동으로 찾음
        image = GetComponent<Image>();

        // 페이드 아웃 효과 시작
        StartCoroutine(FadeInOutRoutine());
    }

    IEnumerator FadeInOutRoutine()
    {
        while (true)
        {
            // 랜덤 시간(1~5초) 설정
            randomTime = Random.Range(3f, 5f);

            // 이미지 사라지기 (페이드 아웃)
            yield return StartCoroutine(FadeOut(fadeDuration));

            // 대기 시간 후 다시 나타나기
            yield return new WaitForSeconds(randomTime);

            // 이미지 나타나기 (페이드 인)
            yield return StartCoroutine(FadeIn(fadeDuration));

            // 대기 시간 후 다시 페이드 아웃 시작
            yield return new WaitForSeconds(randomTime);
        }
    }

    IEnumerator FadeOut(float duration)
    {
        float timeElapsed = 0f;
        Color startColor = image.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (timeElapsed < duration)
        {
            image.color = Color.Lerp(startColor, targetColor, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        image.color = targetColor; // 완전히 투명해지도록 설정
    }

    IEnumerator FadeIn(float duration)
    {
        float timeElapsed = 0f;
        Color startColor = image.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1);

        while (timeElapsed < duration)
        {
            image.color = Color.Lerp(startColor, targetColor, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        image.color = targetColor; // 완전히 불투명해지도록 설정
    }
}
