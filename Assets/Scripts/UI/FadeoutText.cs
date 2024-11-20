//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;

//public class TextFadeOut : MonoBehaviour
//{
//    public Text characterNameText;
//    public float fadeDuration = 3f;

//    private IEnumerator fadeOutCoroutine;

//    void Start()
//    {
//        characterNameText.enabled = false;
//    }

//    public void DisplayErrorMessage(string message, Vector2 position, int fontSize, Color fontColor)
//    {
//        characterNameText.text = message;
//        characterNameText.fontSize = fontSize;
//        characterNameText.rectTransform.anchoredPosition = position;
//        characterNameText.color = fontColor;
//        characterNameText.enabled = true;

//        if (fadeOutCoroutine != null)
//        {
//            StopCoroutine(fadeOutCoroutine);
//        }
//        fadeOutCoroutine = FadeOutText();
//        StartCoroutine(fadeOutCoroutine);
//    }

//    private IEnumerator FadeOutText()
//    {
//        float elapsedTime = 0f;
//        Color textColor = characterNameText.color;

//        while (elapsedTime < fadeDuration)
//        {
//            elapsedTime += Time.deltaTime;
//            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
//            textColor.a = alpha;
//            characterNameText.color = textColor;
//            yield return null;
//        }

//        characterNameText.enabled = false;
//    }
//}