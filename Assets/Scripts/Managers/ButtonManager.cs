using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("버튼 사운드")]
    [SerializeField]
    public Button[] buttonClickSound; // 사운드가 적용될 버튼들
    private SoundManager soundManager;

    [Header("버튼 색상 변경")]
    public Button[] buttons; // 버튼 목록
    public Color originalColor = Color.white; // 원래 버튼 색상
    public Color inactiveColor = new Color(0.8f, 0.8f, 0.8f); // 비활성화된 버튼 색상 (회색)

    private int clickedIndex = -1; // 현재 클릭된 버튼의 인덱스

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();

        // 사운드가 적용될 버튼의 클릭 이벤트 등록
        foreach (var button in buttonClickSound)
        {
            button.onClick.AddListener(PlayButtonClickSound);
        }

        // 색상 변경이 적용될 버튼의 클릭 이벤트 등록
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 클로저 문제 방지
            buttons[i].onClick.AddListener(() => ChangeButtonColor(index));
        }

        // 초기 버튼 색상 설정
        ResetButtonColors();
    }

    // 사운드 재생 메서드
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

    // 버튼 클릭 시 색상 변경 메서드
    private void ChangeButtonColor(int index)
    {
        clickedIndex = index; // 현재 클릭된 버튼의 인덱스 저장

        // 모든 버튼의 색상 변경
        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();
            if (i == clickedIndex)
            {
                // 클릭한 버튼은 원래 색상 유지
                buttonImage.color = originalColor;
            }
            else
            {
                // 나머지 버튼은 회색으로 변경
                buttonImage.color = inactiveColor;
            }
        }
    }

    // 초기 상태로 모든 버튼 색상 설정
    private void ResetButtonColors()
    {
        foreach (var button in buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = originalColor;
        }
    }
}
