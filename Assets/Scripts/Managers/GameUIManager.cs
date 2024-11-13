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
    public Text probabilityText;  // Ȯ���� ǥ���� �ؽ�Ʈ UI

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
        UpdateProbabilityText();  // Ȯ�� �ؽ�Ʈ ������Ʈ �Լ� ȣ��
        StartCoroutine(RefreshProbabilityText());  // Ȯ�� ���� �ֱ������� ������Ʈ
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
            Debug.LogWarning("��ȿ���� ���� �˾�â �ε���");
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

    // Ȯ�� �ؽ�Ʈ ������Ʈ
    private void UpdateProbabilityText()
    {
        // EnhancementManager���� Ȯ�� �迭 ��������
        float[] probabilities = EnhancementManager.I.probabilities;

        if (probabilities == null || probabilities.Length == 0)
        {
            Debug.LogWarning("Ȯ�� �迭�� ��� �ֽ��ϴ�!");
            probabilityText.text = "Ȯ�� �����Ͱ� �����ϴ�.";
            return;
        }

        string probabilityString = "";
        for (int i = 0; i < probabilities.Length; i++)
        {
            // ���� ����
            Color gradeColor = EnhancementManager.I.gradeColors[i % EnhancementManager.I.gradeColors.Length];  // ���� �迭�� ���� ������ ���� ��������
            string hexColor = ColorUtility.ToHtmlStringRGB(gradeColor); // ������ HTML ���ڿ��� ��ȯ

            probabilityString += $"<color=#{hexColor}>Lv{i + 1}  -  {probabilities[i]:F3} % </color>\n";
        }

        probabilityText.text = probabilityString;
    }

    // Ȯ���� �ֱ������� ������Ʈ�ϴ� �ڷ�ƾ �߰�
    private IEnumerator RefreshProbabilityText()
    {
        while (true)
        {
            UpdateProbabilityText();
            yield return new WaitForSeconds(1f);  // 1�ʸ��� ������Ʈ
        }
    }
}
