using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemManager : MonoBehaviour
{
    [Header("강화")]
    [Space]
    [SerializeField] private int maxLevel = 20;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text quantityText;

    [SerializeField] private Text[] levelTexts = new Text[6];
    [SerializeField] private Text[] currentLevelTexts = new Text[6];
    [SerializeField] private Button[] itemSlotButtons = new Button[6];  // enhanceButtons -> itemSlotButtons
    [SerializeField] private Toggle[] gradeToggles = new Toggle[6];
    [SerializeField] private GameObject[] itemPrefabs = new GameObject[6];

    private TowerStats towerStats;
    private SoundManager soundManager;
    private GameObject[] instantiatedItems = new GameObject[6];
    public int[] currentLevels = new int[6];
    public int[] itemGrades = new int[6];
    public bool[] slotOccupied = new bool[6];
    private int buyQuantity = 1;

    public static ItemManager I { get; private set; }

    public readonly float[] successRates = { 95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f, 45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f };
    public readonly float[] probabilities = { 83.894f, 10f, 3f, 1.2f, 0.7f, 0.2f, 0.007f };
    public readonly int[] sellPrices = { 50, 200, 500, 1000, 3000, 5000, 20000 };
    public readonly Color[] gradeColors = { Color.white, new Color(0.7f, 0.9f, 1f), new Color(0.8f, 0.6f, 1f), new Color(1f, 0.6f, 0.8f), new Color(1f, 0.8f, 0.4f), Color.yellow, Color.red };

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);

        towerStats = FindObjectOfType<TowerStats>();
    }

    private void Start()
    {
        InitializeItemSlotButtons();  // 함수명 수정
        SetupRightClickEventForBuyButton();
        soundManager = FindObjectOfType<SoundManager>();
        InvokeRepeating(nameof(UpdateBuyButtonState), 0f, 0.1f);
        buyButton.onClick.AddListener(PurchaseItem);
    }

    private void InitializeItemSlotButtons()  // 함수명 수정
    {
        for (int i = 0; i < itemSlotButtons.Length; i++)
        {
            int index = i;
            itemSlotButtons[index].onClick.AddListener(() => AttemptEnhancement(index));  // 강화 시도 함수 호출
            AddRightClickEventForItemSlotButton(itemSlotButtons[index], index);  // 우클릭 이벤트 처리
            UpdateUIForSlot(index);
        }
    }

    private void AttemptEnhancement(int index)
    {
        if (!slotOccupied[index] || currentLevels[index] >= maxLevel) return;

        int enhancementCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[index]));
        float successRate = successRates[currentLevels[index]];

        if (GameManager.I.Gold < enhancementCost) return;

        bool spent = GameManager.I.SpendGold(enhancementCost);
        if (!spent) return;

        if (Random.Range(0f, 100f) <= successRate)
        {
            currentLevels[index]++;
            soundManager.PlaySoundEffect(3); // 강화 성공
        }
        else
        {
            soundManager.PlaySoundEffect(4); // 강화 실패
        }

        string itemDescription = GetItemDescription(itemGrades[index], currentLevels[index]);
        GameUiManager.I.UpdateItemInfo(index, itemDescription);

        levelTexts[index].text = $"$ {enhancementCost}";

        UpdateUIForSlot(index);
    }

    private void UpdateBuyButtonState() => buyButton.interactable = GameManager.I.Gold >= 300;

    private void PurchaseItem()
    {
        const int itemCost = 300;
        int purchasedQuantity = 0;
        int totalCost = itemCost * buyQuantity;

        if (GameManager.I.Gold < totalCost) return;

        for (int i = 0; i < buyQuantity; i++)
        {
            int emptySlot = FindEmptySlot();

            if (emptySlot == -1) break;

            if (!GameManager.I.SpendGold(itemCost)) break;

            int itemGrade = DetermineItemGrade();
            purchasedQuantity++;

            if (gradeToggles[itemGrade - 1].isOn)
            {
                int sellPrice = sellPrices[itemGrade - 1];
                GameManager.I.AddGold(sellPrice);
                continue; // 판매 후 다음 아이템
            }

            CreateItemInSlot(emptySlot, itemGrade);
        }
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < slotOccupied.Length; i++)
        {
            if (!slotOccupied[i]) return i;
        }
        return -1;
    }

    private int DetermineItemGrade()
    {
        float randomValue = Random.Range(0f, 100f);
        float cumulativeProbability = 0f;
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue < cumulativeProbability) return i + 1;
        }
        return 1;
    }

    private void CreateItemInSlot(int slotIndex, int itemGrade)
    {
        Destroy(instantiatedItems[slotIndex]);

        Transform itemFolder = itemSlotButtons[slotIndex].transform.Find("Item");
        instantiatedItems[slotIndex] = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], itemFolder);
        RectTransform itemRect = instantiatedItems[slotIndex].GetComponent<RectTransform>();
        itemRect.anchoredPosition = Vector2.zero;
        itemRect.localScale = Vector3.one;

        itemGrades[slotIndex] = itemGrade;
        currentLevels[slotIndex] = 1;
        slotOccupied[slotIndex] = true;

        itemSlotButtons[slotIndex].image.color = gradeColors[itemGrade - 1];
        int enhancementCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[slotIndex]));
        levelTexts[slotIndex].text = $"$ {enhancementCost}";

        GameUiManager.I.UpdateItemInfo(slotIndex, GetItemDescription(itemGrade, currentLevels[slotIndex]));
        UpdateUIForSlot(slotIndex);
    }

    private void AddRightClickEventForItemSlotButton(Button button, int index)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((data) =>
        {
            if (((PointerEventData)data).button == PointerEventData.InputButton.Right)
                SellItem(index);
        });
        trigger.triggers.Add(entry);
    }

    private void SetupRightClickEventForBuyButton()
    {
        EventTrigger trigger = buyButton.gameObject.GetComponent<EventTrigger>() ?? buyButton.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((data) =>
        {
            if (((PointerEventData)data).button == PointerEventData.InputButton.Right)
            {
                int[] quantities = { 1, 10, 30, 50 };
                int currentIndex = System.Array.IndexOf(quantities, buyQuantity);
                buyQuantity = quantities[(currentIndex + 1) % quantities.Length];
                UpdateQuantityText();
            }
        });
        trigger.triggers.Add(entry);
    }

    private void UpdateQuantityText() => quantityText.text = $"x{buyQuantity}";

    private void UpdateUIForSlot(int index)
    {
        if (slotOccupied[index])
        {
            currentLevelTexts[index].text = $"Lv. {currentLevels[index]}";
            itemSlotButtons[index].interactable = currentLevels[index] < maxLevel;
        }
        else
        {
            currentLevelTexts[index].text = "";
            itemSlotButtons[index].interactable = false;
        }
    }

    public string GetItemDescription(int grade, int level)
    {
        string gradeName = new[] { "일반", "고급", "희귀", "영웅", "전설", "신화", "유니크" }[grade - 1];
        return $"Lv.{level} {gradeName} 아이템 - 옵션: +{level * grade * 10}%";
    }

    private void SellItem(int index)
    {
        if (!slotOccupied[index]) return;

        int itemGrade = itemGrades[index];
        int sellPrice = sellPrices[itemGrade - 1];
        GameManager.I.AddGold(sellPrice);

        Destroy(instantiatedItems[index]);
        soundManager.PlaySoundEffect(2);
        slotOccupied[index] = false;
        currentLevels[index] = 0;
        itemGrades[index] = 0;

        levelTexts[index].text = "Empty";
        itemSlotButtons[index].image.color = Color.gray;
        UpdateUIForSlot(index);
    }
}
