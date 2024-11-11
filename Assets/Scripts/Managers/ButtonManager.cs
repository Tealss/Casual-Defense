using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("버튼 클릭음")]
    [SerializeField]
    public Button[] buttonClickSound;
    private SoundManager soundManager;

    [Header("버튼 색")]
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
            Debug.LogWarning("SoundManager가 초기화되지 않았습니다.");
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
