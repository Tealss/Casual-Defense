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
        InitializeToggleButtons();
        UpdateGoldUI(GameManager.I.Gold);
        UpdateLifePointsText(GameManager.I.LifePoints, GameManager.I.TotalLifePoints); 
        gameOverPanel.SetActive(false);
        StartCoroutine(UpdateGoldCoroutine());
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

        if (lifePoints <= 10)
        {
            LifeText.color = Color.red;
        }
        else
        {
            LifeText.color = Color.white; 
        }
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
