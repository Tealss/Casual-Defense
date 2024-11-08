using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("��ư ����")]
    [SerializeField]
    public Button[] buttonClickSound; // ���尡 ����� ��ư��
    private SoundManager soundManager;

    [Header("��ư ���� ����")]
    public Button[] buttons; // ��ư ���
    public Color originalColor = Color.white; // ���� ��ư ����
    public Color inactiveColor = new Color(0.8f, 0.8f, 0.8f); // ��Ȱ��ȭ�� ��ư ���� (ȸ��)

    private int clickedIndex = -1; // ���� Ŭ���� ��ư�� �ε���

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();

        // ���尡 ����� ��ư�� Ŭ�� �̺�Ʈ ���
        foreach (var button in buttonClickSound)
        {
            button.onClick.AddListener(PlayButtonClickSound);
        }

        // ���� ������ ����� ��ư�� Ŭ�� �̺�Ʈ ���
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Ŭ���� ���� ����
            buttons[i].onClick.AddListener(() => ChangeButtonColor(index));
        }

        // �ʱ� ��ư ���� ����
        ResetButtonColors();
    }

    // ���� ��� �޼���
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

    // ��ư Ŭ�� �� ���� ���� �޼���
    private void ChangeButtonColor(int index)
    {
        clickedIndex = index; // ���� Ŭ���� ��ư�� �ε��� ����

        // ��� ��ư�� ���� ����
        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();
            if (i == clickedIndex)
            {
                // Ŭ���� ��ư�� ���� ���� ����
                buttonImage.color = originalColor;
            }
            else
            {
                // ������ ��ư�� ȸ������ ����
                buttonImage.color = inactiveColor;
            }
        }
    }

    // �ʱ� ���·� ��� ��ư ���� ����
    private void ResetButtonColors()
    {
        foreach (var button in buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = originalColor;
        }
    }
}
