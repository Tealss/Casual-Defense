using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemManager : MonoBehaviour
{
    public static ItemManager I;

    [Header("Item Configuration")]
    [SerializeField] private int maxLevel = 20;
    [SerializeField] private const int itemCost = 200;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text quantityText;
    [SerializeField] private Text[] levelTexts = new Text[6];
    [SerializeField] private Text[] currentLevelTexts = new Text[6];
    [SerializeField] private Button[] itemSlotButtons = new Button[6];
    [SerializeField] private Toggle[] gradeToggles = new Toggle[6];
    [SerializeField] private GameObject[] itemPrefabs = new GameObject[6];

    public ItemStats itemStats;
    public readonly int[] currentLevels = new int[6];
    public readonly int[] itemGrades = new int[6];
    public readonly bool[] slotOccupied = new bool[6];

    private int buyQuantity = 1;
    private GameObject[] instantiatedItems = new GameObject[6];
    private readonly int[] itemTypesInSlots = new int[6];


    public event Action OnItemStatsChanged;
    public readonly float[] successRates = { 95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f, 45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f };
    public readonly float[] probabilities = { 83.894f, 10f, 3f, 1.2f, 0.7f, 0.2f, 0.007f };
    public readonly int[] sellPrices = { 30, 200, 500, 1000, 3000, 5000, 20000 };
    public readonly Color[] gradeColors = { Color.white, new Color(0.7f, 0.9f, 1f), new Color(0.8f, 0.6f, 1f), new Color(1f, 0.6f, 0.8f), new Color(1f, 0.8f, 0.4f), Color.yellow, Color.red };
    public readonly string[] gradeNames = { "Common", "Uncommon", "Rare", "Unique", "Epic", "Legendary", "Mythic" };

    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        InitializeItemSlotButtons();
        SetupBuyButtonEvents();
        itemStats.itemReset();
        InvokeRepeating(nameof(UpdateBuyButtonState), 0f, 0.1f);
    }

    private void InitializeItemSlotButtons()
    {
        for (int i = 0; i < itemSlotButtons.Length; i++)
        {
            int index = i;

            itemSlotButtons[index].onClick.RemoveAllListeners();
            itemSlotButtons[index].onClick.AddListener(() => AttemptEnhancement(index));
            AddRightClickEventForItemSlotButton(itemSlotButtons[index], index);
            UpdateUIForSlot(index);
        }
    }

    private void AddRightClickEventForItemSlotButton(Button button, int index)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((data) =>
        {
            if (((PointerEventData)data).button == PointerEventData.InputButton.Right)
                SellItem(index);
        });
        trigger.triggers.Add(entry);
    }

    private void SetupBuyButtonEvents()
    {
        buyButton.onClick.AddListener(PurchaseItem);
        EventTrigger trigger = buyButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry rightClickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };

        rightClickEntry.callback.AddListener((data) =>
        {
            if (((PointerEventData)data).button == PointerEventData.InputButton.Right)
            {
                UpdateBuyQuantity();
            }
        });

        trigger.triggers.Add(rightClickEntry);
    }
    private void AttemptEnhancement(int index)
    {
        if (!slotOccupied[index] || currentLevels[index] >= maxLevel) return;

        int enhancementCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[index] + 1));
        float successRate = successRates[currentLevels[index]];

        if (GameManager.I.gold < enhancementCost) return;

        bool spent = GameManager.I.SpendGold(enhancementCost);
        if (!spent) return;

        if (UnityEngine.Random.Range(0, 100) <= successRate)
        {
            currentLevels[index]++;
            SoundManager.I.PlaySoundEffect(3);
        }
        else
        {
            SoundManager.I.PlaySoundEffect(4);
        }

        string itemDescription = GetItemDescription(index, itemGrades[index], currentLevels[index]);
        GameUiManager.I.UpdateItemInfo(index, itemDescription);

        levelTexts[index].text = $"$ {enhancementCost}";
        UpdateItemStats();
        UpdateUIForSlot(index);
    }

    private void PurchaseItem()
    {
        int totalCost = itemCost * buyQuantity;

        if (GameManager.I.gold < totalCost) return;

        for (int i = 0; i < buyQuantity; i++)
        {
            int emptySlot = FindEmptySlot();
            if (emptySlot == -1)
            {
                ShowFullInventoryWarning();
                break;
            }

            if (!GameManager.I.SpendGold(itemCost)) break;

            int itemGrade = DetermineItemGrade();
            if (itemGrade - 1 < gradeToggles.Length && gradeToggles[itemGrade - 1].isOn)
            {
                GameManager.I.AddGold(sellPrices[itemGrade - 1]);
                continue;
            }

           
            CreateItemInSlot(emptySlot, itemGrade);
            ShowGoldDeductionFeedback(itemCost);
        }

        UpdateItemStats();

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
        float randomValue = UnityEngine.Random.Range(0, 100);
        float cumulativeProbability = 0f;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue < cumulativeProbability) return i + 1;
        }
        return 1;
    }
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

    private void CreateItemInSlot(int slotIndex, int itemGrade)
    {
        if (slotIndex < 0 || slotIndex >= instantiatedItems.Length) return;

        int itemTypeIndex = UnityEngine.Random.Range(0, itemPrefabs.Length);
        GameObject selectedItemPrefab = itemPrefabs[itemTypeIndex];

        Transform itemFolder = itemSlotButtons[slotIndex].transform.Find("Item");
        instantiatedItems[slotIndex] = Instantiate(selectedItemPrefab, itemFolder);
        SetupItemTransform(instantiatedItems[slotIndex].GetComponent<RectTransform>());

        itemGrades[slotIndex] = itemGrade;
        currentLevels[slotIndex] = 1;
        slotOccupied[slotIndex] = true;

        UpdateSlotAppearance(slotIndex, itemGrade);
        UpdateSlotInfo(slotIndex, itemTypeIndex, itemGrade);
    }

    private void SetupItemTransform(RectTransform itemRect)
    {
        itemRect.anchoredPosition = Vector2.zero;
        itemRect.localScale = Vector3.one;
    }

    private void UpdateSlotAppearance(int slotIndex, int itemGrade)
    {
        itemSlotButtons[slotIndex].image.color = gradeColors[itemGrade - 1];
        levelTexts[slotIndex].text = "$ " + Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[slotIndex]));
    }

    private void UpdateSlotInfo(int slotIndex, int itemTypeIndex, int itemGrade)
    {
        itemTypesInSlots[slotIndex] = itemTypeIndex;
        GameUiManager.I.UpdateItemInfo(slotIndex, GetItemDescription(slotIndex, itemGrade, currentLevels[slotIndex]));
        UpdateSlotUI(slotIndex);
    }

    private void UpdateSlotUI(int index)
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

    private void UpdateBuyButtonState()
    {
        int totalCost = itemCost * buyQuantity;

        buyButton.interactable = GameManager.I.gold >= totalCost;
    }

    private void UpdateBuyQuantity()
    {
        int[] quantities = { 1, 10, 30, 50 };
        int currentIndex = Array.IndexOf(quantities, buyQuantity);
        buyQuantity = quantities[(currentIndex + 1) % quantities.Length];
        quantityText.text = $"x{buyQuantity}";
    }

    private void ShowFullInventoryWarning()
    {
        RectTransform buttonRectTransform = buyButton.GetComponent<RectTransform>();
        Vector3 buttonPosition = buttonRectTransform.anchoredPosition;
        ShowFeedbackText(buttonPosition, "Full", Color.red);
    }

    private void ShowGoldDeductionFeedback(int amount)
    {
        RectTransform buttonRectTransform = buyButton.GetComponent<RectTransform>();
        Vector3 buttonPosition = buttonRectTransform.anchoredPosition;
        ShowFeedbackText(buttonPosition, $"- {amount}", Color.blue);
    }

    private void ShowFeedbackText(Vector3 position, string text, Color color)
    {
        FadeOutTextUse fadeOutTextSpawner = FindObjectOfType<FadeOutTextUse>();
        fadeOutTextSpawner?.SpawnFadeOutText(position, text, color, true);
    }


    public string GetItemDescription(int slotIndex, int grade, int level)
    {
        int itemType = itemTypesInSlots[slotIndex];
        string gradeName = gradeNames[grade - 1];
        string typeDescription = GetItemTypeDescription(itemType);
        string optionDescription = GetItemOptionDescription(itemType, level, grade);

        return $"Grade : {FormatColoredText(gradeColors[grade - 1], gradeName)}\n" +
               $"Type : {FormatColoredText(gradeColors[grade - 1], typeDescription)}\n" +
               $"Option : {FormatColoredText(gradeColors[grade - 1], optionDescription)}\n\n" +
               $"Upgrade % : {successRates[level - 1]}\nSell Price : $ {sellPrices[grade - 1]}";
    }

    private string GetItemTypeDescription(int itemType)
    {
        string[] descriptions = { "Attack Damage", "Attack Speed", "Attack Range", "Critical Chance", "Critical Damage", "Add Gold" };
        return itemType < descriptions.Length ? descriptions[itemType] : "Unknown";
    }
    private string FormatColoredText(Color color, string text)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
    }

    private string GetItemOptionDescription(int itemType, int level, int grade)
    {
        float effect = level * grade;
        return itemType switch
        {
            0 => $"+ {effect * 20}",
            1 => $"+ {effect * 0.05}",
            2 => $"+ {effect * 0.03}",
            3 => $"+ {effect * 0.4}%",
            4 => $"+ {effect * 0.4}%",
            5 => $"+ {effect * 2}",
            _ => "None",
        };
    }

    public float GetItemTypeEffect(int itemType, int level, int grade)
    {
        switch (itemType)
        {
            case 0: return level * grade * 20f;
            case 1: return level * grade * 0.05f;
            case 2: return level * grade * 0.03f;
            case 3: return level * grade * 0.4f;
            case 4: return level * grade * 0.4f;
            case 5: return level * grade * 2f;
            case 6: return level * grade * 5f;

            default: return 0f;


        }

    }
    public float GetTotalItemEffect(int itemType)
    {
        float totalEffect = 0f;

        for (int i = 0; i < itemTypesInSlots.Length; i++)
        {
            if (slotOccupied[i] && itemTypesInSlots[i] == itemType)
            {
                totalEffect += GetItemTypeEffect(itemType, currentLevels[i], itemGrades[i]);
            }
        }

        return totalEffect;
    }

    private void UpdateItemStats()
    {
        //string logMessage = "Updated Total Item Effects:\n";

        itemStats.itemAttackDamage = 0f;
        itemStats.itemAttackSpeed = 0f;
        itemStats.itemAttackRange = 0f;
        itemStats.itemCriticalChance = 0f;
        itemStats.itemCriticalDamage = 0f;
        itemStats.itemGoldEarnAmount = 0f;

        for (int itemType = 0; itemType <= 5; itemType++)
        {
            float totalEffect = GetTotalItemEffect(itemType);

            switch (itemType)
            {
                case 0: itemStats.itemAttackDamage += totalEffect; break;
                case 1: itemStats.itemAttackSpeed += totalEffect; break;
                case 2: itemStats.itemAttackRange += totalEffect; break;
                case 3: itemStats.itemCriticalChance += totalEffect; break;
                case 4: itemStats.itemCriticalDamage += totalEffect; break;
                case 5: itemStats.itemGoldEarnAmount += totalEffect; break;
            }

            //logMessage += $"{GetItemTypeDescription(itemType)}: Total Effect: {totalEffect}\n";
        }

        itemStats.InitializeBaseStats();
        OnItemStatsChanged?.Invoke();
        //Debug.Log(logMessage);
    }

    private void SellItem(int index)
    {
        if (!slotOccupied[index]) return;

        int itemType = itemTypesInSlots[index];
        int itemGrade = itemGrades[index];
        int itemLevel = currentLevels[index];

        int sellPrice = sellPrices[itemGrade - 1];
        GameManager.I.AddGold(sellPrice);

        if (instantiatedItems[index] != null)
        {
            Destroy(instantiatedItems[index]);
        }

        SoundManager.I.PlaySoundEffect(2);

        slotOccupied[index] = false;
        currentLevels[index] = 0;
        itemGrades[index] = 0;

        levelTexts[index].text = "Empty";
        itemSlotButtons[index].image.color = Color.white;

        UpdateUIForSlot(index);

        Vector3 spawnPosition = GetButtonPositionInCanvas(itemSlotButtons[index]);
        string sellText = $"+ {sellPrice}";
        Color textColor = Color.yellow;

        FadeOutTextUse fadeOutTextSpawner = FindObjectOfType<FadeOutTextUse>();
        if (fadeOutTextSpawner != null)
        {
            fadeOutTextSpawner.SpawnFadeOutText(spawnPosition, sellText, textColor, true);
        }

        UpdateItemStats();
    }

    private Vector3 GetButtonPositionInCanvas(Button button)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        Vector2 anchoredPosition = rectTransform.anchoredPosition;
        return anchoredPosition;
    }



}