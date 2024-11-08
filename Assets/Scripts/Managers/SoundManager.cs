using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("배경음")]
    [SerializeField]
    public List<AudioClip> backgroundMusicClips;
    public Slider backgroundMusicSlider;
    public Button muteBackgroundButton;
    public Image backgroundMuteOffImage;

    [Header("효과음")]
    [SerializeField]
    public List<AudioClip> soundEffects;
    public Slider effectsSoundSlider;
    public Button muteEffectsButton;
    public Image effectsMuteOffImage;

    private AudioSource backgroundAudioSource;
    private AudioSource effectsAudioSource;

    private bool isBackgroundMuted = false;
    private bool isEffectsMuted = false;

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
            Debug.LogWarning("배경음악 리스트가 비어 있습니다.");
        }

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

        if (muteBackgroundButton != null)
        {
            muteBackgroundButton.onClick.AddListener(ToggleBackgroundMute);
        }

        if (muteEffectsButton != null)
        {
            muteEffectsButton.onClick.AddListener(ToggleEffectsMute);
        }

        UpdateBackgroundMuteIcon();
        UpdateEffectsMuteIcon();
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
            Debug.LogWarning("유효하지 않은 배경음악 인덱스입니다.");
        }
    }

    public void SetBackgroundVolume(float volume)
    {
        if (volume == 0f)
        {
            if (!isBackgroundMuted) 
            {
                isBackgroundMuted = true;
                backgroundAudioSource.mute = true;
            }
        }
        else
        {
            if (isBackgroundMuted) 
            {
                isBackgroundMuted = false;
                backgroundAudioSource.mute = false;
            }
            backgroundAudioSource.volume = Mathf.Clamp(volume, 0f, 1f);
        }

        UpdateBackgroundMuteIcon();
    }

    public void SetEffectsVolume(float volume)
    {
        if (volume == 0f) 
        {
            if (!isEffectsMuted) 
            {
                isEffectsMuted = true;
                effectsAudioSource.mute = true;
            }
        }
        else
        {
            if (isEffectsMuted) 
            {
                isEffectsMuted = false;
                effectsAudioSource.mute = false;
            }
            effectsAudioSource.volume = Mathf.Clamp(volume, 0f, 1f);
        }

        UpdateEffectsMuteIcon();
    }

    private void ToggleBackgroundMute()
    {
        isBackgroundMuted = !isBackgroundMuted;
        backgroundAudioSource.mute = isBackgroundMuted;

        if (isBackgroundMuted)
        {
            backgroundAudioSource.volume = 0f;
            backgroundMusicSlider.value = 0f;
        }
        else
        {
            backgroundAudioSource.volume = 1f;
            backgroundMusicSlider.value = 1f;
        }

        UpdateBackgroundMuteIcon();
    }

    private void ToggleEffectsMute()
    {
        isEffectsMuted = !isEffectsMuted;
        effectsAudioSource.mute = isEffectsMuted;

        if (isEffectsMuted)
        {
            effectsAudioSource.volume = 0f;
            effectsSoundSlider.value = 0f;
        }
        else
        {
            effectsAudioSource.volume = 1f;
            effectsSoundSlider.value = 1f;
        }

        UpdateEffectsMuteIcon();
    }

    private void UpdateBackgroundMuteIcon()
    {
        backgroundMuteOffImage.gameObject.SetActive(isBackgroundMuted || backgroundAudioSource.volume == 0);
    }

    private void UpdateEffectsMuteIcon()
    {
        effectsMuteOffImage.gameObject.SetActive(isEffectsMuted || effectsAudioSource.volume == 0);
    }

    public void PlaySoundEffect(int index)
    {
        if (index >= 0 && index < soundEffects.Count)
        {
            effectsAudioSource.PlayOneShot(soundEffects[index]);
        }
        else
        {
            Debug.LogWarning("유효하지 않은 효과음 인덱스입니다.");
        }
    }
}
