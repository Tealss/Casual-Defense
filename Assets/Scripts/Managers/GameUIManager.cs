using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUiManager : MonoBehaviour
{
    public GameObject[] popupWindows;
    public Button[] toggleButtons;
    public Text goldText;

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
        UpdateGoldUI(GameManager.Instance.Gold);
        StartCoroutine(UpdateGoldCoroutine());
    }

    private void InitializeToggleButtons()
    {
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;
            toggleButtons[i].onClick.AddListener(() => TogglePopup(index));
        }
    }

    public void UpdateGoldUI(int gold)
    {
        goldText.text = $" :  {gold:N0}";
    }

    private IEnumerator UpdateGoldCoroutine()
    {
        while (true)
        {
            UpdateGoldUI(GameManager.Instance.Gold);
            yield return new WaitForSeconds(0.1f);
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
