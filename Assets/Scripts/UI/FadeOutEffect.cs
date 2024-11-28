using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutEffect : MonoBehaviour
{
    private Image image; // ���̵� �ƿ� ȿ���� ������ �̹���
    private float fadeDuration = 0.2f; // ���̵� ��/�ƿ� ȿ���� ���� �ð�
    private float randomTime; // ���� �ð�

    void Start()
    {
        // �̹��� ������Ʈ�� �ڵ����� ã��
        image = GetComponent<Image>();

        // ���̵� �ƿ� ȿ�� ����
        StartCoroutine(FadeInOutRoutine());
    }

    IEnumerator FadeInOutRoutine()
    {
        while (true)
        {
            // ���� �ð�(1~5��) ����
            randomTime = Random.Range(3f, 5f);

            // �̹��� ������� (���̵� �ƿ�)
            yield return StartCoroutine(FadeOut(fadeDuration));

            // ��� �ð� �� �ٽ� ��Ÿ����
            yield return new WaitForSeconds(randomTime);

            // �̹��� ��Ÿ���� (���̵� ��)
            yield return StartCoroutine(FadeIn(fadeDuration));

            // ��� �ð� �� �ٽ� ���̵� �ƿ� ����
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

        image.color = targetColor; // ������ ������������ ����
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

        image.color = targetColor; // ������ �������������� ����
    }
}
