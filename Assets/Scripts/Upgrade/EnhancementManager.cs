using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnhancementManager : MonoBehaviour
{
    // �ִ� ��ȭ ����
    public int maxLevel = 20;

    // ��ȭ ������ ���� �迭
    private int[] currentLevels = new int[6];               // �� �������� ���� ��ȭ ����
    private int[] itemGrades = new int[6];                  // �� �������� ���
    public Text[] levelTexts = new Text[6];                 // �� �������� ������ ǥ���� �ؽ�Ʈ (��ȭ ����/���� �޽�����)
    public Text[] currentLevelTexts = new Text[6];          // �� �������� ���� ������ ǥ���� �ؽ�Ʈ
    public Button[] enhanceButtons = new Button[6];         // �� �������� ��ȭ ��ư
    public Toggle[] gradeToggles = new Toggle[6];           // �� �������� ��� ��� ��ư

    // ������ ���� ���� ����
    public Button buyButton;                                // ������ ���� ��ư
    private bool[] slotOccupied = new bool[6];              // �� ���Կ� �������� �ִ��� Ȯ���ϱ� ���� �迭

    // ��ȭ Ȯ�� �迭
    private float[] successRates = {
        95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f,
        45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f
    };

    // ��޿� ���� ��ư ����
    private Color[] gradeColors = {
        Color.white,                 // 1�ܰ�: ���
        new Color(0.7f, 0.9f, 1f),   // 2�ܰ�: �ϴû�
        new Color(0.8f, 0.6f, 1f),   // 3�ܰ�: �����
        new Color(1f, 0.6f, 0.8f),   // 4�ܰ�: ��ȫ��
        new Color(1f, 0.8f, 0.4f),   // 5�ܰ�: ��Ȳ��
        Color.yellow,                // 6�ܰ�: �����
        Color.red                    // 7�ܰ�: ������
    };

    private void Start()
    {
        // �ʱ�ȭ �� �� ��ư�� �̺�Ʈ ����
        for (int i = 0; i < enhanceButtons.Length; i++)
        {
            int index = i; // Ŭ���� ���� ������ ���� ���� ���� ���
            enhanceButtons[i].onClick.AddListener(() => TryEnhancement(index));
            AddRightClickEvent(enhanceButtons[i], index);
            slotOccupied[i] = false;  // ó������ ������ ����ִ� ����
            UpdateUI(index);           // �ʱ� UI ������Ʈ
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

        // ��ȭ ��� ��� (������ ���� 1.5�辿 ����)
        int enhancementCost = Mathf.CeilToInt(100 * Mathf.Pow(1.2f, currentLevels[index]));

        // ��� ���� �� ����
        if (!GameManager.Instance.SpendGold(enhancementCost))
        {
            levelTexts[index].text = $"��� ���� (�ʿ�: {enhancementCost}��)";
            return;
        }

        // ���� ������ ��ȭ ���� Ȯ�� ��������
        float successRate = successRates[currentLevels[index]];
        float randomValue = Random.Range(0f, 100f);

        // ��ȭ ���� ���� ����
        if (randomValue <= successRate)
        {
            currentLevels[index]++;
            levelTexts[index].text = $"���� {currentLevels[index]} (��ȭ ����!)";
        }
        else
        {
            levelTexts[index].text = $"���� {currentLevels[index]} (��ȭ ����)";
        }

        UpdateUI(index); // UI ������Ʈ
    }

    // UI ������Ʈ
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

    // ������ �ʱ�ȭ (�Ǹ� ���)
    private void SellItem(int index)
    {
        if (!slotOccupied[index])
            return;

        // ��޿� ���� �Ǹ� ���� ���
        int sellPrice = 100 * (int)Mathf.Pow(2, itemGrades[index] - 1);

        // �Ǹ� ��� �߰�
        GameManager.Instance.AddGold(sellPrice);

        // �Ǹ� �޽��� ���
        levelTexts[index].text = $"������ �Ǹŵ� (+{sellPrice}��)";

        // ���� �ʱ�ȭ
        slotOccupied[index] = false;
        currentLevels[index] = 0;
        itemGrades[index] = 0;  // ��� �ʱ�ȭ
        enhanceButtons[index].image.color = Color.gray;  // �Ǹ� �� ȸ������ ����
        enhanceButtons[index].interactable = false;  // ��ȭ ��ư ��Ȱ��ȭ

        // UI ������Ʈ
        UpdateUI(index);
    }

    // ���콺 ������ Ŭ�� �̺�Ʈ �߰�
    private void AddRightClickEvent(Button button, int index)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // ���콺 ������ Ŭ�� �̺�Ʈ �߰�
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
        int itemCost = 300;

        // ��� ���� �� ����
        if (!GameManager.Instance.SpendGold(itemCost))
        {
            Debug.Log("��尡 �����մϴ�! (�ʿ�: 300��)");
            return;
        }

        // ù ��° �� ���� ã��
        int emptySlot = -1;
        for (int i = 0; i < slotOccupied.Length; i++)
        {
            if (!slotOccupied[i])
            {
                emptySlot = i;
                break;
            }
        }

        // ��� ������ ���� �� ���
        if (emptySlot == -1)
        {
            Debug.Log("��� ������ ���� á���ϴ�!");

            // ��� ȯ��
            GameManager.Instance.AddGold(itemCost);
            Debug.Log($"���� �Ұ��� {itemCost}�� ȯ��");
            return;
        }

        // ��� ���� (1�ܰ� ��� ~ 6�ܰ� ���������)
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

        itemGrades[emptySlot] = itemGrade; // ���õ� ����� ���Կ� ����

        // ���Կ� ������ ��� UI ������Ʈ
        currentLevels[emptySlot] = 1; // �ʱ� ������ 1�� ����
        slotOccupied[emptySlot] = true;
        enhanceButtons[emptySlot].interactable = true; // ���� �� ��ȭ ��ư Ȱ��ȭ
        enhanceButtons[emptySlot].image.color = gradeColors[itemGrade - 1]; // ��޿� ���� ��ư ���� ����

        for (int i = 0; i < gradeToggles.Length; i++)
        {
            if (gradeToggles[i].isOn && itemGrade == (i + 1))
            {
                SellItem(emptySlot);
                break;
            }
        }

        UpdateUI(emptySlot);

        Debug.Log($"�������� ��� {itemGrade}�� ���� {emptySlot + 1}�� ��ġ�Ǿ����ϴ�.");
    }

}
