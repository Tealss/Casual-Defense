using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnhancementManager : MonoBehaviour
{
    // 최대 강화 레벨
    public int maxLevel = 20;

    // 강화 아이템 관련 배열
    private int[] currentLevels = new int[6];               // 각 아이템의 현재 강화 레벨
    private int[] itemGrades = new int[6];                  // 각 아이템의 등급
    public Text[] levelTexts = new Text[6];                 // 각 아이템의 레벨을 표시할 텍스트 (강화 성공/실패 메시지용)
    public Text[] currentLevelTexts = new Text[6];          // 각 아이템의 현재 레벨을 표시할 텍스트
    public Button[] enhanceButtons = new Button[6];         // 각 아이템의 강화 버튼

    // 아이템 상점 관련 변수
    public Button buyButton;                                // 아이템 구매 버튼
    public Button gradeUpgradeButton;                       // 아이템 등급 업그레이드 버튼
    private bool[] slotOccupied = new bool[6];              // 각 슬롯에 아이템이 있는지 확인하기 위한 배열

    // 초기 확률 배열 (1등급 ~ 7등급)
    private float[] gradeProbabilities = { 75.5f, 15f, 5f, 3f, 1f, 0.43f, 0.07f };

    // 강화 확률 배열
    private float[] successRates = {
        95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f,
        45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f
    };

    // 등급에 따른 버튼 색상
    private Color[] gradeColors = {
        Color.white,                 // 1단계: 옅은 흰색
        new Color(0.7f, 0.9f, 1f),   // 2단계: 하늘색
        new Color(0.8f, 0.6f, 1f),   // 3단계: 보라색
        new Color(1f, 0.6f, 0.8f),   // 4단계: 분홍색
        new Color(1f, 0.8f, 0.4f),   // 5단계: 주황색
        Color.yellow,                // 6단계: 노란색
        Color.red                    // 7단계: 빨간색
    };

    private void Start()
    {
        // 초기화 및 각 버튼에 이벤트 연결
        for (int i = 0; i < enhanceButtons.Length; i++)
        {
            int index = i; // 클로저 문제 방지를 위해 로컬 변수 사용
            enhanceButtons[i].onClick.AddListener(() => TryEnhancement(index));
            AddRightClickEvent(enhanceButtons[i], index);
            slotOccupied[i] = false;  // 처음에는 슬롯이 비어있는 상태
            UpdateUI(index);           // 초기 UI 업데이트
        }

        // 상점 버튼에 아이템 구매 기능 추가
        buyButton.onClick.AddListener(BuyItem);
        gradeUpgradeButton.onClick.AddListener(UpgradeItemGrade); // 새 버튼에 아이템 등급 업그레이드 기능 추가
    }

    // 특정 아이템 칸의 강화 시도
    private void TryEnhancement(int index)
    {
        if (!slotOccupied[index])
        {
            levelTexts[index].text = "아이템 없음";  // 슬롯이 비어있을 경우 강화 불가
            return;
        }

        if (currentLevels[index] >= maxLevel)
        {
            levelTexts[index].text = $"레벨 {currentLevels[index]} (최대)";
            return;
        }

        // 현재 레벨의 강화 성공 확률 가져오기
        float successRate = successRates[currentLevels[index]];
        float randomValue = Random.Range(0f, 100f);

        // 강화 성공 여부 결정
        if (randomValue <= successRate)
        {
            currentLevels[index]++;
            levelTexts[index].text = $"레벨 {currentLevels[index]} (강화 성공!)";
        }
        else
        {
            levelTexts[index].text = $"레벨 {currentLevels[index]} (강화 실패)";
        }

        UpdateUI(index); // UI 업데이트
    }

    // UI 업데이트
    private void UpdateUI(int index)
    {
        if (slotOccupied[index] && currentLevels[index] < maxLevel)
        {
            float nextSuccessRate = successRates[currentLevels[index]];
            enhanceButtons[index].GetComponentInChildren<Text>().text = $"강화하기 ({nextSuccessRate}%)";
            enhanceButtons[index].image.color = gradeColors[itemGrades[index] - 1]; // 등급에 맞는 색상 적용
        }
        else if (!slotOccupied[index])
        {
            enhanceButtons[index].GetComponentInChildren<Text>().text = "아이템 없음";
            enhanceButtons[index].image.color = Color.gray;  // 슬롯이 비어있으면 회색으로 표시
            enhanceButtons[index].interactable = false;  // 버튼 비활성화
        }
        else
        {
            enhanceButtons[index].GetComponentInChildren<Text>().text = "최대 레벨";
            enhanceButtons[index].interactable = false; // 버튼 비활성화
        }

        // 현재 레벨 텍스트 업데이트
        currentLevelTexts[index].text = slotOccupied[index] ? $"Lv. {currentLevels[index]}" : "";
    }

    // 아이템 초기화 (판매 기능)
    private void SellItem(int index)
    {
        slotOccupied[index] = false;
        currentLevels[index] = 0;
        itemGrades[index] = 0;  // 등급 초기화
        levelTexts[index].text = "아이템 판매됨";
        enhanceButtons[index].image.color = Color.gray;  // 판매 후 회색으로 변경
        enhanceButtons[index].interactable = false;  // 강화 버튼 비활성화
        UpdateUI(index);  // UI 업데이트
    }

    // 마우스 오른쪽 클릭 이벤트 추가
    private void AddRightClickEvent(Button button, int index)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // 마우스 오른쪽 클릭 이벤트 추가
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

    // 아이템 구매 함수
    private void BuyItem()
    {
        // 첫 번째 빈 슬롯 찾기
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
            return;
        }

        // 확률을 기반으로 아이템 등급 결정
        int itemGrade = GetRandomItemGrade();

        itemGrades[emptySlot] = itemGrade; // 선택된 등급을 슬롯에 저장

        // 슬롯에 레벨과 등급 UI 업데이트
        currentLevels[emptySlot] = 1; // 초기 레벨은 1로 설정
        slotOccupied[emptySlot] = true;
        enhanceButtons[emptySlot].interactable = true; // 구매 후 강화 버튼 활성화
        enhanceButtons[emptySlot].image.color = gradeColors[itemGrade - 1]; // 등급에 따른 버튼 색상 적용
        UpdateUI(emptySlot);

        Debug.Log($"아이템이 등급 {itemGrade}로 슬롯 {emptySlot + 1}에 배치되었습니다.");
    }

    // 확률을 기반으로 아이템 등급을 결정하는 함수
    private int GetRandomItemGrade()
    {
        // 확률 조정 (높은 등급이 나올 확률을 높임)
        float randomValue = Random.Range(0f, 100f);
        float cumulativeProbability = 0f;

        for (int i = gradeProbabilities.Length - 1; i >= 0; i--)
        {
            cumulativeProbability += gradeProbabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return i + 1;  // 등급은 1부터 시작하므로 i + 1
            }
        }

        return 1;  // 기본적으로 1등급
    }

    // 아이템 등급 업그레이드 함수
    private void UpgradeItemGrade()
    {
        // 첫 번째 빈 슬롯 찾기
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
            return;
        }

        // 아이템을 구매하고 나서, 해당 슬롯의 등급을 업그레이드
        if (itemGrades[emptySlot] < maxLevel)
        {
            itemGrades[emptySlot]++;
            UpdateUI(emptySlot);
        }
        else
        {
            Debug.Log("아이템이 이미 최대 등급입니다.");
        }
    }
}
