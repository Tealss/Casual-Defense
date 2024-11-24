using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FadeOutText : MonoBehaviour
{
    // 이게 지금 페이드아웃 텍스트거든?
    // 이걸 지금 텍스트로 고정시켜놓은거고
    public Text textComponent;
    public float moveSpeed;
    public float fadeDuration;

    private Vector3 moveDirection = new Vector3(1, 1, 0).normalized;

    private Color textColor;

    private void Start()
    {
        textColor = textComponent.color;
        Destroy(gameObject, fadeDuration);
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 페이드아웃 처리
        float fadeAmount = Time.deltaTime / fadeDuration;
        textColor.a -= fadeAmount;
        textComponent.color = textColor;
    }

    public void Initialize(string message, Color color)
    {
        textComponent.text = message;
        textColor = color;
        textComponent.color = textColor;
    }
}