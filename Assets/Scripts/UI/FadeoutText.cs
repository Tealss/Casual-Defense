using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FadeOutText : MonoBehaviour
{
    public Text textComponent;
    public float moveSpeed;
    public float fadeDuration = 0.3f;

    private Vector3 moveDirection = new Vector3(0.7f, 0.8f, 0).normalized;

    private Color textColor;

    private void Start()
    {
        textColor = textComponent.color;
        Destroy(gameObject, fadeDuration);
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

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