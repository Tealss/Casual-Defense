using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("��ư����")]
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
            Debug.LogWarning("SoundManager�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
        }
    }
}