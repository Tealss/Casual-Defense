using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnhancementManager : MonoBehaviour
{
    [Header("��ȭ")]
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
    // ��ȭ ���� Ȯ��
    public float[] successRates = {
        95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f,
        45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f
    };
    // ������ ��� Ȯ��
    public float[] probabilities = { 
        84.193f, 8.5f, 4f, 2.0f, 1.0f, 0.3f, 0.007f 
    };

    // ��ȭ ��޺� ����
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
            levelTexts[index].text = "������ ����";
            return;
        }

        if (currentLevels[index] >= maxLevel)
        {
            levelTexts[index].text = $"���� {currentLevels[index]} (�ִ�)";
            return;
        }

        int enhancementCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[index]));

        if (!GameManager.I.SpendGold(enhancementCost))
        {
            levelTexts[index].text = $"��� ���� (�ʿ�: {enhancementCost}��)";
            return;
        }

        float successRate = successRates[currentLevels[index]];
        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= successRate)
        {
            currentLevels[index]++;
            levelTexts[index].text = $"���� {currentLevels[index]} (��ȭ ����!)";
        }
        else
        {
            levelTexts[index].text = $"���� {currentLevels[index]} (��ȭ ����)";
        }

        UpdateUI(index);
    }

    private void UpdateUI(int index)
    {
        if (slotOccupied[index] && currentLevels[index] < maxLevel)
        {
            int nextCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[index]));
            float nextSuccessRate = successRates[currentLevels[index]];
            enhanceButtons[index].GetComponentInChildren<Text>().text = $"��ȭ�ϱ� ({nextSuccessRate}%, {nextCost}��)";
            enhanceButtons[index].image.color = gradeColors[itemGrades[index] - 1];
        }
        else if (!slotOccupied[index])
        {
            enhanceButtons[index].GetComponentInChildren<Text>().text = "������ ����";
            enhanceButtons[index].image.color = Color.gray;
            enhanceButtons[index].interactable = false;
        }
        else
        {
            enhanceButtons[index].GetComponentInChildren<Text>().text = "�ִ� ����";
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
        levelTexts[index].text = $"������ �Ǹŵ� (+{sellPrice}��)";
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
            Debug.Log("��尡 �����մϴ�! (�ʿ�: 300��)");
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
            Debug.Log("��� ������ ���� á���ϴ�!");
            GameManager.I.AddGold(itemCost);
            Debug.Log($"���� �Ұ��� {itemCost}�� ȯ��");
            return;
        }

        //--------------------------������ ��� Ȯ�� ����ȭ------------------------------

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

        // ������ ��� Ȯ�� ���
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

        // ������ Ÿ�� ���� ���� (6�� Ÿ�� �� ���� ����)
        int numberOfItemTypes = 6;  // 6���� Ÿ��
        float randomValueForType = Random.Range(0f, 1f);  // 0���� 1 ������ ������ ���� ����
        int itemType = Mathf.FloorToInt(randomValueForType * numberOfItemTypes);  // 0~5 ������ ������ Ÿ�� ����

        // ����� �α׷� Ÿ�԰� ��� Ȯ��
        Debug.Log($"������ Ÿ�� {itemType + 1} ����, ������ ��� {itemGrade}");

        itemGrades[emptySlot] = itemGrade;  // ������ ��� ����
        currentLevels[emptySlot] = 1;
        slotOccupied[emptySlot] = true;
        enhanceButtons[emptySlot].interactable = true;
        enhanceButtons[emptySlot].image.color = gradeColors[itemType];  // ������ Ÿ�Կ� �´� ���� ����

        for (int i = 0; i < gradeToggles.Length; i++)
        {
            if (gradeToggles[i].isOn && itemGrades[emptySlot] == (i + 1))
            {
                SellItem(emptySlot);
                break;
            }
        }

        UpdateUI(emptySlot);
        Debug.Log($"�������� Ÿ�� {itemType + 1}��, ��� {itemGrade}�� ���� {emptySlot + 1}�� ��ġ�Ǿ����ϴ�.");
    }

}
