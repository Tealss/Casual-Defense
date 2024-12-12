using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GameUiManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject[] popupWindows;
    public Button[] popupWindowButtons;
    public Button[] bountyButtons;

    [Header("Text UI")]
    public Text infoText;
    public Text itemInfoText;

    public GameObject fadeOutTextPrefab;
    public Canvas parentCanvas;

    public Text waveText;
    public Text timerText;
    public Text lifeText;
    public Text totalLifeText;
    public Text goldText;

    [Header("Player UI")]
    public Text playerLevelText;
    public Slider playerExpSlider;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    public static GameUiManager I;

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        for (int i = 0; i < bountyButtons.Length; i++)
        {
            int index = i;
            bountyButtons[index].onClick.AddListener(() => OnBountyButtonClicked(index));
        }

        UpdateGoldUI(GameManager.I.gold);
        UpdateLifePointsText(GameManager.I.lifePoints, GameManager.I.totalLifePoints);
        UpdatePlayerUI(GameManager.I.playerLevel, GameManager.I.playerExperience, GameManager.I.experienceToNextLevel);
        gameOverPanel.SetActive(false);
        StartCoroutine(UpdateGoldCoroutine());

        AddEventTriggerToPopupWindowButtons();
        UpdateProbabilityText();
        StartCoroutine(RefreshProbabilityText());
    }

    public void OnBountyButtonClicked(int index)
    {
        if (WaveManager.I != null)
        {
            WaveManager.I.SpawnBountyMonster(index + 1);
        }
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
        }
    }

    public void UpdatePlayerUI(int level, int currentExp, int expToNextLevel)
    {
        playerLevelText.text = $"Lv. {level}";
        playerExpSlider.maxValue = expToNextLevel;
        playerExpSlider.value = currentExp;
    }

    public void OnPointerEnter(int index)
    {
        if (index >= 0 && index <= 5)
        {
            popupWindows[0].SetActive(true);
            string itemDescription = GetItemDescriptionFromItemManager(index);
            UpdateItemInfo(index, itemDescription);
        }
        else if (index == 6)
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

        itemInfoText.text = string.Empty;
    }

    private string GetItemDescriptionFromItemManager(int slotIndex)
    {
        if (ItemManager.I.slotOccupied[slotIndex])
        {
            int itemGrade = ItemManager.I.itemGrades[slotIndex];
            int itemLevel = ItemManager.I.currentLevels[slotIndex];
            return ItemManager.I.GetItemDescription(slotIndex, itemGrade, itemLevel);
        }
        return "    Empty Slot";
    }

    public void UpdateItemInfo(int slotIndex, string itemDescription)
    {
        switch (slotIndex)
        {
            case 0:
                itemInfoText.text = $"{itemDescription}";
                break;
            case 1:
                itemInfoText.text = $"{itemDescription}";
                break;
            case 2:
                itemInfoText.text = $"{itemDescription}";
                break;
            case 3:
                itemInfoText.text = $"{itemDescription}";
                break;
            case 4:
                itemInfoText.text = $"{itemDescription}";
                break;
            case 5:
                itemInfoText.text = $"{itemDescription}";
                break;
            default:
                itemInfoText.text = "Unknown slot.";
                break;
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
        lifeText.text = $"{lifePoints}";
        totalLifeText.text = $"{totalLifePoints}";
        lifeText.color = lifePoints <= 10 ? Color.red : Color.white;
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
            UpdateGoldUI(GameManager.I.gold);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateProbabilityText()
    {
        float[] probabilities = ItemManager.I.probabilities;

        if (probabilities == null || probabilities.Length == 0)
        {
            Debug.LogWarning("No probability data available");
            infoText.text = "No probability data.";
            return;
        }

        string probabilityString = "";
        for (int i = 0; i < probabilities.Length; i++)
        {
            Color gradeColor = ItemManager.I.gradeColors[i % ItemManager.I.gradeColors.Length];
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
