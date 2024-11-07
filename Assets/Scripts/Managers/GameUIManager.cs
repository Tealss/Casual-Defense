using UnityEngine;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour
{
    [Header("팝업창")]
    public GameObject[] popupWindows; 

    [Header("버튼")]
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
            Debug.LogWarning("유효하지 않은 팝업 창 인덱스");
        }
    }
}