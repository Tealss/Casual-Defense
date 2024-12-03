using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("BGM")]
    [Space]
    public List<AudioClip> backgroundMusicClips;
    public Slider backgroundMusicSlider;
    public Button muteBackgroundButton;
    public Image backgroundMuteOffImage;

    [Header("SFX")]
    [Space]
    public List<AudioClip> soundEffects;
    public Slider effectsSoundSlider;
    public Button muteEffectsButton;
    public Image effectsMuteOffImage;

    public static SoundManager I { get; private set; }
    public AudioSource BackgroundAudioSource => backgroundAudioSource;
    private AudioSource backgroundAudioSource;
    private AudioSource effectsAudioSource;

    private bool isBackgroundMuted = false;
    private bool isEffectsMuted = false;

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }
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
            Debug.LogWarning("The list is empty");
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
    public void PlayBackgroundMusic(int index)
    {
        if (index >= 0 && index < backgroundMusicClips.Count)
        {
            backgroundAudioSource.clip = backgroundMusicClips[index];


            if (index == 0)
            {
                backgroundAudioSource.volume = 0.8f;
            }
            else
            {
                backgroundAudioSource.volume = 1.0f;
            }

            backgroundAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("Bgm is null");
        }
    }

    public void PlaySoundEffect(int index)
    {
        if (index >= 0 && index < soundEffects.Count)
        {

            float baseVolume;

            if (index == 12 || index == 13)
            {
                baseVolume = 0.15f;
            }
            else if (index >= 3)
            {
                baseVolume = 0.25f;
            }
            else
            {
                baseVolume = 1.0f;
            }

            float sliderAdjustedVolume = effectsAudioSource.volume;

            float finalVolume = baseVolume * sliderAdjustedVolume;

            effectsAudioSource.PlayOneShot(soundEffects[index], finalVolume);
        }
        else
        {
            Debug.LogWarning("Sfx is null");
        }
    }

}
