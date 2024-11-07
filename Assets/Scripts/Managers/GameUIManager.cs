using UnityEngine;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour
{
    [Header("�˾�â")]
    public GameObject[] popupWindows; 

    [Header("��ư")]
    public Button[] toggleButtons; 

    void Start()
    {
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;  
            toggleButtons[i].onClick.AddListener(() => TogglePopup(index));
        }
    }

    private void TogglePopup(int index)
    {
        if (index >= 0 && index < popupWindows.Length)
        {
            bool isActive = popupWindows[index].activeSelf;
            popupWindows[index].SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("��ȿ���� ���� �˾� â �ε���");
        }
    }
}