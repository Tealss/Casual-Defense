using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GameUiManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    public GameObject[] popupWindows;
    public GameObject[] pointWindows;
    public Button[] toggleButtons;
    public Button ShopButton;

    [Header("Text UI")]
    [SerializeField]
    public Text waveText;
    public Text timerText;
    public Text LifeText;
    public Text TotalLifeText;
    public Text goldText;
    public Text probabilityText;  // 확률을 표시할 텍스트 UI

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    public static GameUiManager I { get; private set; }

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        pointWindows[0].SetActive(false);
        InitializeToggleButtons();
        UpdateGoldUI(GameManager.I.Gold);
        UpdateLifePointsText(GameManager.I.LifePoints, GameManager.I.TotalLifePoints);
        gameOverPanel.SetActive(false);
        StartCoroutine(UpdateGoldCoroutine());

        AddEventTriggerToShopButton();
        UpdateProbabilityText();  // 확률 텍스트 업데이트 함수 호출
        StartCoroutine(RefreshProbabilityText());  // 확률 값을 주기적으로 업데이트
    }

    private void AddEventTriggerToShopButton()
    {
        EventTrigger eventTrigger = ShopButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnterEntry.callback.AddListener((eventData) => OnPointerEnter());
        eventTrigger.triggers.Add(pointerEnterEntry);

        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExitEntry.callback.AddListener((eventData) => OnPointerExit());
        eventTrigger.triggers.Add(pointerExitEntry);
    }

    public void UpdateWaveText(int waveNumber)
    {
        waveText.text = $"{waveNumber}   -";
    }

    public void UpdateTimerText(int secondsLeft)
    {
        int minutes = secondsLeft / 60;
        int seconds = secondsLeft % 60;
        timerText.text = $"{minutes:00} : {seconds:00}";
    }

    public void UpdateLifePointsText(int lifePoints, int totalLifePoints)
    {
        LifeText.text = $"{lifePoints}";
        TotalLifeText.text = $"{totalLifePoints}";
        LifeText.color = lifePoints <= 10 ? Color.red : Color.white;
    }

    public void UpdateGoldUI(int gold)
    {
        goldText.text = $" :  {gold:N0}";
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    private IEnumerator UpdateGoldCoroutine()
    {
        while (true)
        {
            UpdateGoldUI(GameManager.I.Gold);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void InitializeToggleButtons()
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
            popupWindows[index].SetActive(!popupWindows[index].activeSelf);
        }
        else
        {
            Debug.LogWarning("유효하지 않은 팝업창 인덱스");
        }
    }

    public void OnPointerEnter()
    {
        pointWindows[0].SetActive(true);
    }

    public void OnPointerExit()
    {
        pointWindows[0].SetActive(false);
    }

    // 확률 텍스트 업데이트
    private void UpdateProbabilityText()
    {
        // EnhancementManager에서 확률 배열 가져오기
        float[] probabilities = EnhancementManager.I.probabilities;

        if (probabilities == null || probabilities.Length == 0)
        {
            Debug.LogWarning("확률 배열이 비어 있습니다!");
            probabilityText.text = "확률 데이터가 없습니다.";
            return;
        }

        string probabilityString = "";
        for (int i = 0; i < probabilities.Length; i++)
        {
            // 색상 적용
            Color gradeColor = EnhancementManager.I.gradeColors[i % EnhancementManager.I.gradeColors.Length];  // 색상 배열의 범위 내에서 색상 가져오기
            string hexColor = ColorUtility.ToHtmlStringRGB(gradeColor); // 색상을 HTML 문자열로 변환

            probabilityString += $"<color=#{hexColor}>Lv{i + 1}  -  {probabilities[i]:F3} % </color>\n";
        }

        probabilityText.text = probabilityString;
    }

    // 확률을 주기적으로 업데이트하는 코루틴 추가
    private IEnumerator RefreshProbabilityText()
    {
        while (true)
        {
            UpdateProbabilityText();
            yield return new WaitForSeconds(1f);  // 1초마다 업데이트
        }
    }
}
