using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnhancementManager : MonoBehaviour
{
    [Header("강화")]
    [SerializeField]
    public int maxLevel = 20;
    public Button buyButton;
    public Text[] levelTexts = new Text[6];
    public Text[] currentLevelTexts = new Text[6];
    public Button[] enhanceButtons = new Button[6];
    public Toggle[] gradeToggles = new Toggle[6];

    private int[] currentLevels = new int[6];
    private int[] itemGrades = new int[6];
    private bool[] slotOccupied = new bool[6];

    public static EnhancementManager I { get; private set; }
    // 강화 성공 확률
    public float[] successRates = {
        95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f,
        45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f
    };
    // 아이템 등급 확률
    public float[] probabilities = { 
        84.193f, 8.5f, 4f, 2.0f, 1.0f, 0.3f, 0.007f 
    };

    // 강화 등급별 색상
    public Color[] gradeColors = {
        Color.white,
        new Color(0.7f, 0.9f, 1f),
        new Color(0.8f, 0.6f, 1f),
        new Color(1f, 0.6f, 0.8f),
        new Color(1f, 0.8f, 0.4f),
        Color.yellow,
        Color.red
    };

    private void Awake()
    {
        if (I== null)
            I = this;
        else
            Destroy(gameObject);
    }

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

        if (!GameManager.I.SpendGold(enhancementCost))
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
        GameManager.I.AddGold(sellPrice);
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
        if (GameManager.I.Gold >= 300)
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
        int itemCost = 300;

        if (!GameManager.I.SpendGold(itemCost))
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
            GameManager.I.AddGold(itemCost);
            Debug.Log($"구매 불가능 {itemCost}원 환불");
            return;
        }

        //--------------------------아이템 등급 확률 정규화------------------------------

        float totalProbability = 0f;
        foreach (var probability in probabilities)
        {
            totalProbability += probability;
        }

        if (totalProbability > 100f)
        {
            for (int i = 0; i < probabilities.Length; i++)
            {
                probabilities[i] = probabilities[i] * 100f / totalProbability;
            }
        }

        // 아이템 등급 확률 계산
        float cumulativeProbability = 0f;
        float randomValueForGrade = Random.Range(0f, 100f);
        int itemGrade = 1;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValueForGrade < cumulativeProbability)
            {
                itemGrade = i + 1;
                break;
            }
        }

        //---------------------------------------------------------------------

        // 아이템 타입 랜덤 생성 (6개 타입 중 랜덤 선택)
        int numberOfItemTypes = 6;  // 6가지 타입
        float randomValueForType = Random.Range(0f, 1f);  // 0부터 1 사이의 값으로 랜덤 생성
        int itemType = Mathf.FloorToInt(randomValueForType * numberOfItemTypes);  // 0~5 사이의 값으로 타입 선택

        // 디버그 로그로 타입과 등급 확인
        Debug.Log($"아이템 타입 {itemType + 1} 생성, 아이템 등급 {itemGrade}");

        itemGrades[emptySlot] = itemGrade;  // 아이템 등급 설정
        currentLevels[emptySlot] = 1;
        slotOccupied[emptySlot] = true;
        enhanceButtons[emptySlot].interactable = true;
        enhanceButtons[emptySlot].image.color = gradeColors[itemType];  // 아이템 타입에 맞는 색상 적용

        for (int i = 0; i < gradeToggles.Length; i++)
        {
            if (gradeToggles[i].isOn && itemGrades[emptySlot] == (i + 1))
            {
                SellItem(emptySlot);
                break;
            }
        }

        UpdateUI(emptySlot);
        Debug.Log($"아이템이 타입 {itemType + 1}로, 등급 {itemGrade}로 슬롯 {emptySlot + 1}에 배치되었습니다.");
    }

}
