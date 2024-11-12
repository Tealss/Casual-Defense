using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUiManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    public GameObject[] popupWindows;
    public Button[] toggleButtons;

    [Header("Text UI")]
    [SerializeField]
    public Text waveText;
    public Text timerText;
    public Text LifeText;
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
        InitializeToggleButtons();
        UpdateGoldUI(GameManager.I.Gold);
        UpdateLifePointsText(GameManager.I.LifePoints);
        gameOverPanel.SetActive(false);
        StartCoroutine(UpdateGoldCoroutine());
    }

    public void UpdateWaveText(int waveNumber)
    {
        waveText.text = $"Wave: {waveNumber}";
    }

    public void UpdateTimerText(int secondsLeft)
    {
        timerText.text = $"Time: {secondsLeft:00}";
    }

    public void UpdateLifePointsText(int lifePoints)
    {
        LifeText.text = $"Life: {lifePoints}";
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
}
