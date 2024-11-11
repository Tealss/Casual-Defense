using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("��ư Ŭ����")]
    [SerializeField]
    public Button[] buttonClickSound;
    private SoundManager soundManager;

    [Header("��ư ��")]
    [SerializeField]
    public Button[] buttons;
    public Color originalColor = Color.white;
    public Color inactiveColor = new Color(0.8f, 0.8f, 0.8f);

    private int clickedIndex = -1;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();

        foreach (var button in buttonClickSound)
        {
            button.onClick.AddListener(PlayButtonClickSound);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => ChangeButtonColor(index));
        }

        InitializeButtonColors();
    }

    private void PlayButtonClickSound()
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

    private void ChangeButtonColor(int index)
    {
        clickedIndex = index;

        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();
            if (i == clickedIndex)
            {
                buttonImage.color = originalColor;
            }
            else
            {
                buttonImage.color = inactiveColor;
            }
        }
    }

    private void InitializeButtonColors()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();

            if (i == 0)
            {
                buttonImage.color = originalColor;
            }
            else
            {
                buttonImage.color = inactiveColor;
            }
        }
    }
}
