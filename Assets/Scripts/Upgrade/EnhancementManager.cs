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

    // ������ ���� ���� ����
    public Button buyButton;                                // ������ ���� ��ư
    public Button gradeUpgradeButton;                       // ������ ��� ���׷��̵� ��ư
    private bool[] slotOccupied = new bool[6];              // �� ���Կ� �������� �ִ��� Ȯ���ϱ� ���� �迭

    // �ʱ� Ȯ�� �迭 (1��� ~ 7���)
    private float[] gradeProbabilities = { 75.5f, 15f, 5f, 3f, 1f, 0.43f, 0.07f };

    // ��ȭ Ȯ�� �迭
    private float[] successRates = {
        95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f,
        45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f
    };

    // ��޿� ���� ��ư ����
    private Color[] gradeColors = {
        Color.white,                 // 1�ܰ�: ���� ���
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

        // ���� ��ư�� ������ ���� ��� �߰�
        buyButton.onClick.AddListener(BuyItem);
        gradeUpgradeButton.onClick.AddListener(UpgradeItemGrade); // �� ��ư�� ������ ��� ���׷��̵� ��� �߰�
    }

    // Ư�� ������ ĭ�� ��ȭ �õ�
    private void TryEnhancement(int index)
    {
        if (!slotOccupied[index])
        {
            levelTexts[index].text = "������ ����";  // ������ ������� ��� ��ȭ �Ұ�
            return;
        }

        if (currentLevels[index] >= maxLevel)
        {
            levelTexts[index].text = $"���� {currentLevels[index]} (�ִ�)";
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
            float nextSuccessRate = successRates[currentLevels[index]];
            enhanceButtons[index].GetComponentInChildren<Text>().text = $"��ȭ�ϱ� ({nextSuccessRate}%)";
            enhanceButtons[index].image.color = gradeColors[itemGrades[index] - 1]; // ��޿� �´� ���� ����
        }
        else if (!slotOccupied[index])
        {
            enhanceButtons[index].GetComponentInChildren<Text>().text = "������ ����";
            enhanceButtons[index].image.color = Color.gray;  // ������ ��������� ȸ������ ǥ��
            enhanceButtons[index].interactable = false;  // ��ư ��Ȱ��ȭ
        }
        else
        {
            enhanceButtons[index].GetComponentInChildren<Text>().text = "�ִ� ����";
            enhanceButtons[index].interactable = false; // ��ư ��Ȱ��ȭ
        }

        // ���� ���� �ؽ�Ʈ ������Ʈ
        currentLevelTexts[index].text = slotOccupied[index] ? $"Lv. {currentLevels[index]}" : "";
    }

    // ������ �ʱ�ȭ (�Ǹ� ���)
    private void SellItem(int index)
    {
        slotOccupied[index] = false;
        currentLevels[index] = 0;
        itemGrades[index] = 0;  // ��� �ʱ�ȭ
        levelTexts[index].text = "������ �Ǹŵ�";
        enhanceButtons[index].image.color = Color.gray;  // �Ǹ� �� ȸ������ ����
        enhanceButtons[index].interactable = false;  // ��ȭ ��ư ��Ȱ��ȭ
        UpdateUI(index);  // UI ������Ʈ
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

    // ������ ���� �Լ�
    private void BuyItem()
    {
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

        if (emptySlot == -1)
        {
            Debug.Log("��� ������ ���� á���ϴ�!");
            return;
        }

        // Ȯ���� ������� ������ ��� ����
        int itemGrade = GetRandomItemGrade();

        itemGrades[emptySlot] = itemGrade; // ���õ� ����� ���Կ� ����

        // ���Կ� ������ ��� UI ������Ʈ
        currentLevels[emptySlot] = 1; // �ʱ� ������ 1�� ����
        slotOccupied[emptySlot] = true;
        enhanceButtons[emptySlot].interactable = true; // ���� �� ��ȭ ��ư Ȱ��ȭ
        enhanceButtons[emptySlot].image.color = gradeColors[itemGrade - 1]; // ��޿� ���� ��ư ���� ����
        UpdateUI(emptySlot);

        Debug.Log($"�������� ��� {itemGrade}�� ���� {emptySlot + 1}�� ��ġ�Ǿ����ϴ�.");
    }

    // Ȯ���� ������� ������ ����� �����ϴ� �Լ�
    private int GetRandomItemGrade()
    {
        // Ȯ�� ���� (���� ����� ���� Ȯ���� ����)
        float randomValue = Random.Range(0f, 100f);
        float cumulativeProbability = 0f;

        for (int i = gradeProbabilities.Length - 1; i >= 0; i--)
        {
            cumulativeProbability += gradeProbabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return i + 1;  // ����� 1���� �����ϹǷ� i + 1
            }
        }

        return 1;  // �⺻������ 1���
    }

    // ������ ��� ���׷��̵� �Լ�
    private void UpgradeItemGrade()
    {
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

        if (emptySlot == -1)
        {
            Debug.Log("��� ������ ���� á���ϴ�!");
            return;
        }

        // �������� �����ϰ� ����, �ش� ������ ����� ���׷��̵�
        if (itemGrades[emptySlot] < maxLevel)
        {
            itemGrades[emptySlot]++;
            UpdateUI(emptySlot);
        }
        else
        {
            Debug.Log("�������� �̹� �ִ� ����Դϴ�.");
        }
    }
}
