using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public Text blinkingText;
    public GameObject[] setFalseObject;
    public GameObject[] setTrueObject;
    public GameObject[] gameManagers;

    private bool canProcessInput = false;

    void Start()
    {
        StartCoroutine(EnableInputAfterDelay(2f));
        StartCoroutine(BlinkText());
    }

    void Update()
    {
        if (canProcessInput && Input.anyKeyDown && !GameManager.I.hasTransitioned)
        {
            GameManager.I.hasTransitioned = true;

            if (setFalseObject != null)
            {
                for (int i = 0; i < setFalseObject.Length; i++)
                {
                    setFalseObject[i].SetActive(false);
                }
            }

            if (setTrueObject != null)
            {
                StartCoroutine(ActivateObjectsSequentially());
            }

            GameManager.I.isBlinking = false;
            blinkingText.enabled = true;

            SoundManager.I.PlaySoundEffect(10);
            StartCoroutine(FadeOutAndIn(0f, 1f));
        }
    }

    private IEnumerator EnableInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canProcessInput = true; // 2초 후 입력 활성화
    }

    private IEnumerator BlinkText()
    {
        while (GameManager.I.isBlinking)
        {
            blinkingText.enabled = !blinkingText.enabled;
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator ActivateObjectsSequentially()
    {
        for (int i = 0; i < Mathf.Min(2, setTrueObject.Length); i++)
        {
            setTrueObject[i].SetActive(true);
        }

        for (int i = 2; i < setTrueObject.Length - 1; i++)
        {
            StartCoroutine(FadeInObject(setTrueObject[i]));
            yield return new WaitForSeconds(0.45f);
        }

        for (int i = setTrueObject.Length - 1; i < setTrueObject.Length; i++)
        {
            yield return new WaitForSeconds(1.00f);
            setTrueObject[i].SetActive(true);

        }

        if (gameManagers != null)
        {
            foreach (GameObject manager in gameManagers)
            {
                if (manager != null)
                {
                    manager.SetActive(true);
                }
            }
        }
    }

    private IEnumerator FadeInObject(GameObject obj)
    {
        float duration = 1.5f;
        float elapsedTime = 0f;

        obj.SetActive(true);
        Transform objTransform = obj.transform;

        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        objTransform.localScale = initialScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            objTransform.localScale = Vector3.Lerp(initialScale, targetScale, progress);

            yield return null;
        }

        objTransform.localScale = targetScale;
    }

    private IEnumerator FadeOutAndIn(float fadeOutVolume, float fadeInVolume)
    {
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        AudioSource backgroundAudioSource = SoundManager.I.BackgroundAudioSource;
        float initialVolume = backgroundAudioSource.volume;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float volume = Mathf.Lerp(initialVolume, fadeOutVolume, elapsedTime / fadeDuration);
            backgroundAudioSource.volume = volume;
            yield return null;
        }

        backgroundAudioSource.Stop();

        SoundManager.I.PlayBackgroundMusic(1);

        backgroundAudioSource = SoundManager.I.BackgroundAudioSource;
        backgroundAudioSource.volume = 0f;

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float volume = Mathf.Lerp(0f, fadeInVolume, elapsedTime / fadeDuration);
            backgroundAudioSource.volume = volume;
            yield return null;
        }

        backgroundAudioSource.volume = fadeInVolume;
    }
}
