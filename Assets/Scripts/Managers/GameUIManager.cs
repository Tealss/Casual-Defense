using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GameUiManager : MonoBehaviour
{
    [Header("UI")]
    [Space]
    public GameObject[] popupWindows;
    public Button[] popupWindowButtons;

    [Header("Text UI")]
    [Space]
    public Text infoText;
    public Text itemInfoText;

    public Text waveText;
    public Text timerText;
    public Text LifeText;
    public Text TotalLifeText;
    public Text goldText;

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
        UpdateGoldUI(GameManager.I.Gold);
        UpdateLifePointsText(GameManager.I.LifePoints, GameManager.I.TotalLifePoints);
        gameOverPanel.SetActive(false);
        StartCoroutine(UpdateGoldCoroutine());

        AddEventTriggerToPopupWindowButtons();
        UpdateProbabilityText();
        StartCoroutine(RefreshProbabilityText());
    }

    private void AddEventTriggerToPopupWindowButtons()
    {
        for (int i = 0; i < popupWindowButtons.Length; i++)
        {
            int index = i;
            EventTrigger eventTrigger = popupWindowButtons[index].gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEnterEntry.callback.AddListener((eventData) => OnPointerEnter(index));
            eventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            pointerExitEntry.callback.AddListener((eventData) => OnPointerExit(index));
            eventTrigger.triggers.Add(pointerExitEntry);

            popupWindowButtons[index].onClick.AddListener(() => TogglePopup(index));
        }
    }

    // Pointer Enter event handler (마우스가 버튼 위로 올라왔을 때)
    public void OnPointerEnter(int index)
    {
        if (index >= 0 && index <= 5) // 아이템 버튼만 처리 (슬롯 0~5)
        {
            popupWindows[0].SetActive(true);
            string itemDescription = GetItemDescriptionFromEnhancementManager(index);
            UpdateItemInfo(index, itemDescription);
        }
        else if (index == 6) // 팝업 윈도우 처리
        {
            popupWindows[1].SetActive(true);
        }
    }

    public void OnPointerExit(int index)
    {
        if (index >= 0 && index <= 5)
        {
            popupWindows[0].SetActive(false);
        }
        else if (index == 6)
        {
            popupWindows[1].SetActive(false);
        }

        // 아이템 정보 초기화
        itemInfoText.text = string.Empty;
    }
    private string GetItemDescriptionFromEnhancementManager(int slotIndex)
    {
        if (EnhancementManager.I.slotOccupied[slotIndex])
        {
            int itemGrade = EnhancementManager.I.itemGrades[slotIndex];
            int itemLevel = EnhancementManager.I.currentLevels[slotIndex];
            return EnhancementManager.I.GetItemDescription(itemGrade, itemLevel); // EnhancementManager의 GetItemDescription 사용
        }
        return "빈 슬롯"; // 슬롯이 비어 있으면 "빈 슬롯" 텍스트 표시
    }

    public void UpdateItemInfo(int slotIndex, string itemDescription)
    {

        // 슬롯에 따라 UI 업데이트
        switch (slotIndex)
        {
            case 0:
                itemInfoText.text = $"Slot 1: {itemDescription}";
                break;
            case 1:
                itemInfoText.text = $"Slot 2: {itemDescription}";
                break;
            case 2:
                itemInfoText.text = $"Slot 3: {itemDescription}";
                break;
            case 3:
                itemInfoText.text = $"Slot 4: {itemDescription}";
                break;
            case 4:
                itemInfoText.text = $"Slot 5: {itemDescription}";
                break;
            case 5:
                itemInfoText.text = $"Slot 6: {itemDescription}";
                break;
            default:
                itemInfoText.text = "Unknown slot.";
                break;
        }
    }

    private void TogglePopup(int index)
    {
        if (index >= 0 && index < popupWindows.Length)
        {
            popupWindows[index].SetActive(!popupWindows[index].activeSelf);
        }
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

    private void UpdateProbabilityText()
    {
        float[] probabilities = EnhancementManager.I.probabilities;

        if (probabilities == null || probabilities.Length == 0)
        {
            Debug.LogWarning("No probability data available");
            infoText.text = "No probability data.";
            return;
        }

        string probabilityString = "";
        for (int i = 0; i < probabilities.Length; i++)
        {
            Color gradeColor = EnhancementManager.I.gradeColors[i % EnhancementManager.I.gradeColors.Length];
            string hexColor = ColorUtility.ToHtmlStringRGB(gradeColor);

            probabilityString += $"<color=#{hexColor}>Lv. {i + 1}   -  {probabilities[i]:F3} % </color>\n";
        }

        infoText.text = probabilityString;
    }

    private IEnumerator RefreshProbabilityText()
    {
        while (true)
        {
            UpdateProbabilityText();
            yield return new WaitForSeconds(1f);
        }
    }
}
