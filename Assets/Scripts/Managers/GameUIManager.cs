using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GameUiManager : MonoBehaviour
{
    [Header("UI")]
    public Button optionBtn;
    public GameObject optionPanel;

    public GameObject[] popupWindows;
    public Button[] popupWindowButtons;
    public Button[] bountyButtons;

    [Header("Game Control Buttons")]
    public Button[] gameOverButtons;
    public Button[] gameRestartButtons;

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
    public Slider playerExpSlider2;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public GameObject newRecord;

    public Text overLevelText;
    public Text overPlayTime;
    public Text overBestWave;
    public Text overcurrentWave;
    public Text overExp;

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
        // 기존 로직
        for (int i = 0; i < bountyButtons.Length; i++)
        {
            int index = i;
            bountyButtons[index].onClick.AddListener(() => OnBountyButtonClicked(index));
        }

        foreach (Button button in gameOverButtons)
        {
            button.onClick.AddListener(GameOver);
        }

        foreach (Button button in gameRestartButtons)
        {
            button.onClick.AddListener(RestartGame);
        }

        optionBtn.onClick.AddListener(ToggleOptionPanel);

        UpdateGoldUI(GameManager.I.gold);
        UpdateLifePointsText(GameManager.I.lifePoints, GameManager.I.totalLifePoints);
        UpdatePlayerUI(GameManager.I.playerLevel, GameManager.I.playerExperience, GameManager.I.experienceToNextLevel);
        gameOverPanel.SetActive(false);
        StartCoroutine(UpdateGoldCoroutine());

        AddEventTriggerToPopupWindowButtons();
        UpdateProbabilityText();
        StartCoroutine(RefreshProbabilityText());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionPanel();
        }
    }
    private void ToggleOptionPanel()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(!optionPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Option Panel is not assigned.");
        }
    }

    public void OnBountyButtonClicked(int index)
    {
        if (WaveManager.I != null)
        {
            WaveManager.I.SpawnBountyMonster(index);
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

        playerExpSlider = playerExpSlider2;
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
        int exp = GameManager.I.expScore;
        gameOverPanel.SetActive(true);

        overLevelText.text = ($"{GameManager.I.playerLevel}");
        overBestWave.text = ($"Best Wave : {GameManager.I.bestWave}");
        overcurrentWave.text = ($"Wave : {WaveManager.I.currentWave}");
        overExp.text = ($"{GameManager.I.playerExperience}/{GameManager.I.experienceToNextLevel} <color=#FFFF00> Exp + {exp}</color>");

        int playTimeInSeconds = GameManager.I.playTime;
        int minutes = playTimeInSeconds / 60;
        int seconds = playTimeInSeconds % 60;
        string formattedPlayTime = string.Format(" Play time - {0:00} : {1:00}", minutes, seconds);

        overPlayTime.text = formattedPlayTime;


        UpdatePlayerUI(GameManager.I.playerLevel, GameManager.I.playerExperience, GameManager.I.experienceToNextLevel);

    }

    private void GameOver()
    {
        Debug.Log("Game Over triggered.");
        Application.Quit();
    }

    private void RestartGame()
    {
        Debug.Log("Restarting Game...");

        GameManager.I.isBlinking = true;
        GameManager.I.hasTransitioned = false;

        Time.timeScale = 1;

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
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