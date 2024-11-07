using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("버튼사운드")]
    [SerializeField]
    public Button[] buttonClickSound;
    private SoundManager soundManager;

    void Start()
    {

        soundManager = FindObjectOfType<SoundManager>();

        foreach (var button in buttonClickSound)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    public void OnButtonClick()
    {
        if (soundManager != null)
        {
            soundManager.PlaySoundEffect(0);  
        }
        else
        {
            Debug.LogWarning("SoundManager가 초기화되지 않았습니다.");
        }
    }
}