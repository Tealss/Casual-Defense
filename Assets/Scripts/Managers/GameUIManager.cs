using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUiManager : MonoBehaviour
{
    [Header("팝업창")]
    public GameObject[] popupWindows;
    public Button[] toggleButtons;

    [Header("텍스트")]
    public Text goldText;

    public static GameUiManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;
            toggleButtons[i].onClick.AddListener(() => TogglePopup(index));
        }

        // 골드 UI 초기화 및 업데이트 코루틴 시작
        UpdateGoldUI(GameManager.Instance.Gold);
        StartCoroutine(UpdateGoldCoroutine());
    }

    // 골드 UI 갱신 메서드
    public void UpdateGoldUI(int gold)
    {
        goldText.text = $" : {gold}";
    }

    // 골드 UI를 매 초마다 갱신하는 코루틴
    private IEnumerator UpdateGoldCoroutine()
    {
        while (true)
        {
            // GameManager의 골드 값을 가져와서 UI 업데이트
            UpdateGoldUI(GameManager.Instance.Gold);

            // 1초 대기
            yield return new WaitForSeconds(0.1f);
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
