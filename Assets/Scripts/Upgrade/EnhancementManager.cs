using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnhancementManager : MonoBehaviour
{
    [Header("강화")]
    [SerializeField]
    public int maxLevel = 20;
    public Text[] levelTexts = new Text[6];
    public Text[] currentLevelTexts = new Text[6];
    public Button[] enhanceButtons = new Button[6];
    public Toggle[] gradeToggles = new Toggle[6];
    public Button buyButton;

    private int[] currentLevels = new int[6];
    private int[] itemGrades = new int[6];
    private bool[] slotOccupied = new bool[6];

    // 강화 성공 확률
    private float[] successRates = {
        95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f,
        45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f
    };

    // 강화 등급별 색상
    private Color[] gradeColors = {
        Color.white,
        new Color(0.7f, 0.9f, 1f),
        new Color(0.8f, 0.6f, 1f),
        new Color(1f, 0.6f, 0.8f),
        new Color(1f, 0.8f, 0.4f),
        Color.yellow,
        Color.red
    };

    private void Start()
    {
        for (int i = 0; i < enhanceButtons.Length; i++)
        {
            int index = i;
            enhanceButtons[i].onClick.AddListener(() => TryEnhancement(index));
            AddRightClickEvent(enhanceButtons[i], index);
            slotOccupied[i] = false;
            UpdateUI(index);
        }

        UpdateBuyButtonState();
        buyButton.onClick.AddListener(BuyItem);
    }

    private void Update()
    {
        InvokeRepeating("UpdateBuyButtonState", 0f, 0.1f);
    }

    private void TryEnhancement(int index)
    {
        if (!slotOccupied[index])
        {
            levelTexts[index].text = "아이템 없음";
            return;
        }

        if (currentLevels[index] >= maxLevel)
        {
            levelTexts[index].text = $"레벨 {currentLevels[index]} (최대)";
            return;
        }

        int enhancementCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[index]));

        if (!GameManager.Instance.SpendGold(enhancementCost))
        {
            levelTexts[index].text = $"골드 부족 (필요: {enhancementCost}원)";
            return;
        }

        float successRate = successRates[currentLevels[index]];
        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= successRate)
        {
            currentLevels[index]++;
            levelTexts[index].text = $"레벨 {currentLevels[index]} (강화 성공!)";
        }
        else
        {
            levelTexts[index].text = $"레벨 {currentLevels[index]} (강화 실패)";
        }

        UpdateUI(index);
    }

    private void UpdateUI(int index)
    {
        if (slotOccupied[index] && currentLevels[index] < maxLevel)
        {
            int nextCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[index]));
            float nextSuccessRate = successRates[currentLevels[index]];
            enhanceButtons[index].GetComponentInChildren<Text>().text = $"강화하기 ({nextSuccessRate}%, {nextCost}원)";
            enhanceButtons[index].image.color = gradeColors[itemGrades[index] - 1];
        }
        else if (!slotOccupied[index])
        {
            enhanceButtons[index].GetComponentInChildren<Text>().text = "아이템 없음";
            enhanceButtons[index].image.color = Color.gray;
            enhanceButtons[index].interactable = false;
        }
        else
        {
            enhanceButtons[index].GetComponentInChildren<Text>().text = "최대 레벨";
            enhanceButtons[index].interactable = false;
        }

        currentLevelTexts[index].text = slotOccupied[index] ? $"Lv. {currentLevels[index]}" : "";
    }

    private void SellItem(int index)
    {
        if (!slotOccupied[index])
            return;

        int sellPrice = 100 * (int)Mathf.Pow(2, itemGrades[index] - 1);
        GameManager.Instance.AddGold(sellPrice);
        levelTexts[index].text = $"아이템 판매됨 (+{sellPrice}원)";
        slotOccupied[index] = false;
        currentLevels[index] = 0;
        itemGrades[index] = 0;
        enhanceButtons[index].image.color = Color.gray;
        enhanceButtons[index].interactable = false;
        UpdateUI(index);
    }

    private void AddRightClickEvent(Button button, int index)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) =>
        {
            PointerEventData pointerData = (PointerEventData)data;
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                SellItem(index);
            }
        });

        trigger.triggers.Add(entry);
    }

    private void UpdateBuyButtonState()
    {
        if (GameManager.Instance.Gold >= 300)
        {
            buyButton.interactable = true;
        }
        else
        {
            buyButton.interactable = false;
        }
    }

    private void BuyItem()
    {
        //GameUiManager.I.StartMessageAnimation(buyButton.transform.position);

        int itemCost = 300;

        if (!GameManager.Instance.SpendGold(itemCost))
        {
            Debug.Log("골드가 부족합니다! (필요: 300원)");
            return;
        }

        int emptySlot = -1;
        for (int i = 0; i < slotOccupied.Length; i++)
        {
            if (!slotOccupied[i])
            {
                emptySlot = i;
                break;
            }
        }

        if (emptySlot == -1)
        {
            Debug.Log("모든 슬롯이 가득 찼습니다!");
            GameManager.Instance.AddGold(itemCost);
            Debug.Log($"구매 불가능 {itemCost}원 환불");
            return;
        }

        int itemGrade = Random.Range(0f, 100f) switch
        {
            <= 80.5f => 1,
            <= 90.5f => 2,
            <= 96.5f => 3,
            <= 98.5f => 4,
            <= 99.5f => 5,
            <= 99.93f => 6,
            _ => 7
        };

        itemGrades[emptySlot] = itemGrade;
        currentLevels[emptySlot] = 1;
        slotOccupied[emptySlot] = true;
        enhanceButtons[emptySlot].interactable = true;
        enhanceButtons[emptySlot].image.color = gradeColors[itemGrade - 1];

        for (int i = 0; i < gradeToggles.Length; i++)
        {
            if (gradeToggles[i].isOn && itemGrade == (i + 1))
            {
                SellItem(emptySlot);
                break;
            }
        }

        UpdateUI(emptySlot);
        Debug.Log($"아이템이 등급 {itemGrade}로 슬롯 {emptySlot + 1}에 배치되었습니다.");
    }
}
