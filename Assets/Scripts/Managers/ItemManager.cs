using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using static UnityEngine.GraphicsBuffer;

public class ItemManager : MonoBehaviour
{
    [Header("강화")]
    [Space]
    [SerializeField] private int maxLevel = 20;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text quantityText;

    [SerializeField] private Text[] levelTexts = new Text[6];
    [SerializeField] private Text[] currentLevelTexts = new Text[6];
    [SerializeField] private Button[] itemSlotButtons = new Button[6];
    [SerializeField] private Toggle[] gradeToggles = new Toggle[6];
    [SerializeField] private GameObject[] itemPrefabs = new GameObject[6];

    //private TowerStats towerStats;
    public int[] itemTypesInSlots = new int[6];
    public int[] currentLevels = new int[6];
    public int[] itemGrades = new int[6];
    public bool[] slotOccupied = new bool[6];
    private GameObject[] instantiatedItems = new GameObject[6];
    private int buyQuantity = 1;
    public event Action OnItemStatsChanged;

    public static ItemManager I { get; private set; }

    public readonly float[] successRates = { 95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f, 45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f };
    public readonly float[] probabilities = { 83.894f, 10f, 3f, 1.2f, 0.7f, 0.2f, 0.007f };
    public readonly int[] sellPrices = { 30, 200, 500, 1000, 3000, 5000, 20000 };
    public readonly Color[] gradeColors = { Color.white, new Color(0.7f, 0.9f, 1f), new Color(0.8f, 0.6f, 1f), new Color(1f, 0.6f, 0.8f), new Color(1f, 0.8f, 0.4f), Color.yellow, Color.red };

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);   
    }

    private void Start()
    {
        InitializeItemSlotButtons();  
        SetupRightClickEventForBuyButton();
        InvokeRepeating(nameof(UpdateBuyButtonState), 0f, 0.1f);
        buyButton.onClick.AddListener(PurchaseItem);
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
    private void CreateItemInSlot(int slotIndex, int itemGrade)
    {
        if (slotIndex < 0 || slotIndex >= instantiatedItems.Length)
        {
            Debug.LogError($"Invalid slotIndex: {slotIndex}");
            return;
        }

        //itemPrefabs.Length
        int itemTypeIndex = UnityEngine.Random.Range(0, 0);

        if (itemTypeIndex < 0 || itemTypeIndex >= itemPrefabs.Length)
        {
            Debug.LogError($"Invalid itemTypeIndex: {itemTypeIndex}");
            return;
        }

        GameObject selectedItemPrefab = itemPrefabs[itemTypeIndex];

        Transform itemFolder = itemSlotButtons[slotIndex].transform.Find("Item");

        instantiatedItems[slotIndex] = Instantiate(selectedItemPrefab, itemFolder);
        RectTransform itemRect = instantiatedItems[slotIndex].GetComponent<RectTransform>();
        itemRect.anchoredPosition = Vector2.zero;
        itemRect.localScale = Vector3.one;

        itemGrades[slotIndex] = itemGrade;
        currentLevels[slotIndex] = 1;
        slotOccupied[slotIndex] = true;

        itemSlotButtons[slotIndex].image.color = gradeColors[itemGrade - 1];

        int enhancementCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[slotIndex]));
        levelTexts[slotIndex].text = $"$ {enhancementCost}";

        GameUiManager.I.UpdateItemInfo(slotIndex, GetItemDescription(slotIndex, itemGrade, currentLevels[slotIndex]));

        itemTypesInSlots[slotIndex] = itemTypeIndex;
        NotifyItemStatsChanged();
        UpdateUIForSlot(slotIndex);
    }

    private void AttemptEnhancement(int index)
    {
        if (!slotOccupied[index] || currentLevels[index] >= maxLevel) return;

        int enhancementCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[index]));
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
        NotifyItemStatsChanged();
        UpdateUIForSlot(index);
    }


    private void UpdateBuyButtonState() => buyButton.interactable = GameManager.I.gold >= 200;

    private void PurchaseItem()
    {
        const int itemCost = 200;
        int purchasedQuantity = 0;
        int totalCost = itemCost * buyQuantity;

        // Check if there's enough gold
        if (GameManager.I.gold < totalCost) return;

        for (int i = 0; i < buyQuantity; i++)
        {
            int emptySlot = FindEmptySlot();

            if (emptySlot == -1)
            {
                Vector3 spawnPosition2 = GetButtonPositionInCanvas(buyButton) + new Vector3(1.5f, 2f, 0);
                string fullText = "Full";
                Color fullTextColor = Color.red;

                FadeOutTextUse fadeOutTextSpawner2 = FindObjectOfType<FadeOutTextUse>();
                if (fadeOutTextSpawner2 != null)
                {
                    fadeOutTextSpawner2.SpawnFadeOutText(spawnPosition2, fullText, fullTextColor, true);
                }

                break;
            }

            if (!GameManager.I.SpendGold(itemCost)) break;

            int itemGrade = DetermineItemGrade();

            if (itemGrade < 1 || itemGrade > sellPrices.Length)
            {
                Debug.LogError($"Invalid itemGrade: {itemGrade}");
                continue;
            }

            purchasedQuantity++;

            if (itemGrade - 1 < gradeToggles.Length && gradeToggles[itemGrade - 1].isOn)
            {
                int sellPrice = sellPrices[itemGrade - 1];
                GameManager.I.AddGold(sellPrice);
                continue;
            }


            CreateItemInSlot(emptySlot, itemGrade);

            Vector3 spawnPosition = GetButtonPositionInCanvas(buyButton);
            string damageText = $"- {itemCost}";
            Color textColor = Color.blue;

            FadeOutTextUse fadeOutTextSpawner = FindObjectOfType<FadeOutTextUse>();
            if (fadeOutTextSpawner != null)
            {
                fadeOutTextSpawner.SpawnFadeOutText(spawnPosition, damageText, textColor, true);
            }
        }
    }


    private Vector3 GetButtonPositionInCanvas(Button button)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        Vector2 anchoredPosition = rectTransform.anchoredPosition;
        return anchoredPosition;
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

    public string GetItemDescription(int slotIndex, int grade, int level)
    {
        int itemType = itemTypesInSlots[slotIndex];

        string gradeName = new[] { "Common", "Uncommon", "Rare", "Unique", "Epic", "Legendary", "Mythic" }[grade - 1];

        string typeDescription = GetItemTypeDescription(itemType);
        string optionDescription = GetItemTypeOption(itemType, level, grade);

        float successRate = successRates[level - 1]; 
        Color gradeColor = gradeColors[grade - 1];
        int sellPrice = sellPrices[grade - 1];
        string coloredGradeName = $"<color=#{ColorUtility.ToHtmlStringRGBA(gradeColor)}>{gradeName}</color>";
        string coloredTypeDescription = $"<color=#{ColorUtility.ToHtmlStringRGBA(gradeColor)}>{typeDescription}</color>";
        string coloredoptionDescription = $"<color=#{ColorUtility.ToHtmlStringRGBA(gradeColor)}>{optionDescription}</color>";

        return $" Grade : {coloredGradeName} \n * {coloredTypeDescription} \n Option : {coloredoptionDescription} \n\n Upgrade% : {successRate}\n Sell Price - $ {sellPrice}";
    }


    private string GetItemTypeDescription(int itemType)
    {
        switch (itemType)
        {
            case 0: return "Attack Damage";
            case 1: return "Attack Speed";
            case 2: return "Attack Range";
            case 3: return "Critical Chance";
            case 4: return "Critical Damage";
            case 5: return "Add Gold";
            case 6: return "7";
            default: return "Unknown";
        }
    }

    private string GetItemTypeOption(int itemType, int level, int grade)
    {
        switch (itemType)
        {
            case 0: return $" + {level * grade * 10}"; // 공격력
            case 1: return $" + {level * grade * 0.05}"; // 스피드
            case 2: return $" + {level * grade * 0.1}"; // 범위
            case 3: return $" + {level * grade * 0.5}%"; // 크확
            case 4: return $" + {level * grade * 0.5}%"; // 크뎀
            case 5: return $" + {level * grade * 3}"; // 골드
            case 6: return $" + {level * grade * 5}%"; // 임시

            default: return "None";
        }
    }

    public float GetItemTypeEffect(int itemType, int level, int grade)
    {
        switch (itemType)
        {
            case 0: return level * grade * 10f;
            case 1: return level * grade * 0.05f;
            case 2: return level * grade * 0.1f;
            case 3: return level * grade * 0.5f;
            case 4: return level * grade * 0.5f;
            case 5: return level * grade * 3f;
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

        NotifyItemStatsChanged();
        UpdateUIForSlot(index);

        Vector3 spawnPosition = GetButtonPositionInCanvas(itemSlotButtons[index]);
        string sellText = $"+ {sellPrice}";
        Color textColor = Color.yellow;

        FadeOutTextUse fadeOutTextSpawner = FindObjectOfType<FadeOutTextUse>();
        if (fadeOutTextSpawner != null)
        {
           fadeOutTextSpawner.SpawnFadeOutText(spawnPosition, sellText, textColor, true);
        }

    }

    private void NotifyItemStatsChanged()
    {
        OnItemStatsChanged?.Invoke();
    }
}
