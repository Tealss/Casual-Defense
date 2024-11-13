using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOutText : MonoBehaviour
{
    [Header("����")]
    public float fadeDuration = 2f; // ���̵� �ƿ��� �ɸ��� �ð� (��)
    public float moveSpeed = 50f; // �ؽ�Ʈ�� �̵��� �ӵ� (���� �̵��ϴ� �ӵ�)

    private Text textComponent;
    private Vector3 initialPosition;
    private Color initialColor;

    void Start()
    {
        textComponent = GetComponent<Text>();
        initialPosition = transform.position;
        initialColor = textComponent.color;

        // ���̵� �ƿ� �ڷ�ƾ ����
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // �ؽ�Ʈ�� alpha ���� ���� ���ҽ�Ű�� �̵�
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            textComponent.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            // �ؽ�Ʈ�� ���� �̵�
            transform.position = initialPosition + Vector3.up * moveSpeed * (elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���������� ������ ������� �ϰ� �ؽ�Ʈ�� ��Ȱ��ȭ
        textComponent.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        gameObject.SetActive(false); // �ؽ�Ʈ�� ��Ȱ��ȭ
    }
}