using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnhancementManager : MonoBehaviour
{
    [Header("강화")]
    [SerializeField] private int maxLevel = 20;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text quantityText;
    [SerializeField] private Text[] fadeOutTexts;
    [SerializeField] private Text[] levelTexts = new Text[6];
    [SerializeField] private Text[] currentLevelTexts = new Text[6];
    [SerializeField] private Button[] enhanceButtons = new Button[6];
    [SerializeField] private Toggle[] gradeToggles = new Toggle[6];
    [SerializeField] private GameObject[] itemPrefabs = new GameObject[6];

    private GameObject[] instantiatedItems = new GameObject[6];
    private int[] currentLevels = new int[6];
    private int[] itemGrades = new int[6];
    private bool[] slotOccupied = new bool[6];
    private int buyQuantity = 1;

    public static EnhancementManager I { get; private set; }

    public readonly float[] successRates = { 95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f, 45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f };
    public readonly float[] probabilities = { 83.894f, 10f, 3f, 1.2f, 0.7f, 0.2f, 0.007f };
    public readonly int[] sellPrices = { 100, 200, 1000, 3000, 10000, 20000, 50000 };
    public readonly Color[] gradeColors = { Color.white, new Color(0.7f, 0.9f, 1f), new Color(0.8f, 0.6f, 1f), new Color(1f, 0.6f, 0.8f), new Color(1f, 0.8f, 0.4f), Color.yellow, Color.red };

    private void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeEnhanceButtons();
        ShopRightClickEvent(buyButton);
        InvokeRepeating(nameof(UpdateBuyButtonState), 0f, 0.1f);
        buyButton.onClick.AddListener(BuyItem);
    }

    private void InitializeEnhanceButtons()
    {
        for (int i = 0; i < enhanceButtons.Length; i++)
        {
            int index = i;
            enhanceButtons[index].onClick.AddListener(() => TryEnhancement(index));
            AddRightClickEvent(enhanceButtons[index], index);
            UpdateUI(index);
        }
    }

    private void TryEnhancement(int index)
    {
        if (!slotOccupied[index] || currentLevels[index] >= maxLevel) return;

        int enhancementCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[index]));
        float successRate = successRates[currentLevels[index]];
        
        if (Random.Range(0f, 100f) <= successRate) currentLevels[index]++;

        levelTexts[index].text = $"$ {enhancementCost}";
        UpdateUI(index);
    }

    private void UpdateBuyButtonState() => buyButton.interactable = GameManager.I.Gold >= 300;

    private void BuyItem()
    {
        const int itemCost = 300;
        int purchasedQuantity = 0; 


        int totalCost = itemCost * buyQuantity;
        if (GameManager.I.Gold < totalCost)
        {
            Debug.Log("골드가 부족합니다!");
            return;
        }

        for (int i = 0; i < buyQuantity; i++)
        {
            int emptySlot = FindEmptySlot();


            if (emptySlot == -1)
            {
                Debug.Log("모든 슬롯이 가득 찼습니다! 추가 구매 중단");
                break;
            }

            if (!GameManager.I.SpendGold(itemCost)) break;

            int itemGrade = DetermineItemGrade();
            purchasedQuantity++;

            if (gradeToggles[itemGrade - 1].isOn)
            {
                int sellPrice = sellPrices[itemGrade - 1]; 
                GameManager.I.AddGold(sellPrice); 
                Debug.Log($"아이템 등급 {itemGrade}가 즉시 판매되었습니다. {sellPrice} 골드 반환");
                continue; // 판매 후 다음 아이템으로
            }

        
            CreateItem(emptySlot, itemGrade);
        }

        // 페이드아웃 용
        //Debug.Log($"{purchasedQuantity}개의 아이템을 성공적으로 구매했습니다. 총 비용: {purchasedQuantity * itemCost} 골드");
    }


    private int FindEmptySlot()
    {
        for (int i = 0; i < slotOccupied.Length; i++)
            if (!slotOccupied[i]) return i;
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

    private void CreateItem(int slotIndex, int itemGrade)
    {
        Destroy(instantiatedItems[slotIndex]);
        Transform itemFolder = enhanceButtons[slotIndex].transform.Find("Item");
        instantiatedItems[slotIndex] = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], itemFolder);
        
        RectTransform itemRect = instantiatedItems[slotIndex].GetComponent<RectTransform>();
        itemRect.anchoredPosition = Vector2.zero;
        itemRect.localScale = Vector3.one;

        itemGrades[slotIndex] = itemGrade;
        currentLevels[slotIndex] = 1;
        slotOccupied[slotIndex] = true;

        enhanceButtons[slotIndex].image.color = gradeColors[itemGrade - 1];
        UpdateUI(slotIndex);
    }

    private void AddRightClickEvent(Button button, int index)
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

    private void ShopRightClickEvent(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
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

    private void UpdateUI(int index)
    {
        if (slotOccupied[index])
        {
            currentLevelTexts[index].text = $"Lv. {currentLevels[index]}";
            enhanceButtons[index].interactable = currentLevels[index] < maxLevel;
        }
        else
        {
            currentLevelTexts[index].text = "";
            enhanceButtons[index].interactable = false;
        }
    }

    private void SellItem(int index)
    {
        if (!slotOccupied[index]) return;

        int itemGrade = itemGrades[index];
        int sellPrice = sellPrices[itemGrade - 1];
        GameManager.I.AddGold(sellPrice);

        //Debug.Log($"아이템 등급 {itemGrade}를 판매하여 {sellPrice} 골드를 획득했습니다.");

        // 아이템 삭제
        Destroy(instantiatedItems[index]);
        slotOccupied[index] = false;
        currentLevels[index] = 0;
        itemGrades[index] = 0;

        levelTexts[index].text = "Empty";
        enhanceButtons[index].image.color = Color.gray;
        UpdateUI(index);
    }
}
