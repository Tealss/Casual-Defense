using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("�����")]
    [SerializeField]

    public List<AudioClip> backgroundMusicClips;
    public Slider backgroundMusicSlider;

    [Header("ȿ����")]
    [SerializeField]

    public List<AudioClip> soundEffects;
    public Slider effectsSoundSlider;

    private AudioSource backgroundAudioSource;
    private AudioSource effectsAudioSource;

    void Start()
    {
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource.loop = true;

        effectsAudioSource = gameObject.AddComponent<AudioSource>();

        if (backgroundMusicClips != null && backgroundMusicClips.Count > 0)
        {
            PlayBackgroundMusic(0);
        }
        else
        {
            Debug.LogWarning("������� ����Ʈ�� ��� �ֽ��ϴ�.");
        }

        // �����̴� �ʱⰪ �� �̺�Ʈ ����
        if (backgroundMusicSlider != null)
        {
            backgroundMusicSlider.value = backgroundAudioSource.volume;
            backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundVolume);
        }

        if (effectsSoundSlider != null)
        {
            effectsSoundSlider.value = effectsAudioSource.volume;
            effectsSoundSlider.onValueChanged.AddListener(SetEffectsVolume);
        }
    }

    public void PlayBackgroundMusic(int index)
    {
        if (index >= 0 && index < backgroundMusicClips.Count)
        {
            backgroundAudioSource.clip = backgroundMusicClips[index];
            backgroundAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("��ȿ���� ���� ������� �ε����Դϴ�.");
        }
    }

    public void PlayRandomBackgroundMusic()
    {
        if (backgroundMusicClips.Count > 0)
        {
            int randomIndex = Random.Range(0, backgroundMusicClips.Count);
            PlayBackgroundMusic(randomIndex);
        }
        else
        {
            Debug.LogWarning("������� ����Ʈ�� ��� �ֽ��ϴ�.");
        }
    }

    public void StopBackgroundMusic()
    {
        backgroundAudioSource.Stop();
    }

    public void SetBackgroundVolume(float volume)
    {
        backgroundAudioSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    public void SetEffectsVolume(float volume)
    {
        effectsAudioSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    public void PlaySoundEffect(int index)
    {
        if (index >= 0 && index < soundEffects.Count)
        {
            effectsAudioSource.PlayOneShot(soundEffects[index]);
        }
        else
        {
            Debug.LogWarning("��ȿ���� ���� ȿ���� �ε����Դϴ�.");
        }
    }

    public void PlayRandomSoundEffect()
    {
        if (soundEffects.Count > 0)
        {
            int randomIndex = Random.Range(0, soundEffects.Count);
            effectsAudioSource.PlayOneShot(soundEffects[randomIndex]);
        }
        else
        {
            Debug.LogWarning("ȿ���� ����Ʈ�� ��� �ֽ��ϴ�.");
        }
    }
}
