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
    public Toggle[] gradeToggles = new Toggle[6];           // 각 아이템의 등급 토글 버튼

    // 아이템 상점 관련 변수
    public Button buyButton;                                // 아이템 구매 버튼
    private bool[] slotOccupied = new bool[6];              // 각 슬롯에 아이템이 있는지 확인하기 위한 배열

    // 강화 확률 배열
    private float[] successRates = {
        95f, 90f, 85f, 80f, 75f, 70f, 65f, 60f, 55f, 50f,
        45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f, 5f, 3f
    };

    // 등급에 따른 버튼 색상
    private Color[] gradeColors = {
        Color.white,                 // 1단계: 흰색
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
    }

    // 특정 아이템 칸의 강화 시도
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

        // 강화 비용 계산 (레벨에 따라 2배씩 증가)
        int enhancementCost = 100 * (int)Mathf.Pow(2, currentLevels[index]);

        // 골드 검증 및 차감
        if (!GameManager.Instance.SpendGold(enhancementCost))
        {
            levelTexts[index].text = $"골드 부족 (필요: {enhancementCost}원)";
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
            int nextCost = 100 * (int)Mathf.Pow(2, currentLevels[index]);
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

    // 아이템 초기화 (판매 기능)
    private void SellItem(int index)
    {
        if (!slotOccupied[index])
            return;

        // 등급에 따른 판매 가격 계산
        int sellPrice = 100 * (int)Mathf.Pow(2, itemGrades[index] - 1);

        // 판매 대금 추가
        GameManager.Instance.AddGold(sellPrice);

        // 판매 메시지 출력
        levelTexts[index].text = $"아이템 판매됨 (+{sellPrice}원)";

        // 슬롯 초기화
        slotOccupied[index] = false;
        currentLevels[index] = 0;
        itemGrades[index] = 0;  // 등급 초기화
        enhanceButtons[index].image.color = Color.gray;  // 판매 후 회색으로 변경
        enhanceButtons[index].interactable = false;  // 강화 버튼 비활성화

        // UI 업데이트
        UpdateUI(index);
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
        int itemCost = 300;

        // 골드 검증 및 차감
        if (!GameManager.Instance.SpendGold(itemCost))
        {
            Debug.Log("골드가 부족합니다! (필요: 300원)");
            return;
        }

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

        // 등급 설정 (1단계 흰색 ~ 6단계 노란색까지)
        float randomValue = Random.Range(0f, 100f);
        int itemGrade;

        // 각 등급에 따른 확률 범위
        if (randomValue <= 75.5f)       // 1단계: 흰색, 75.5% 확률
        {
            itemGrade = 1;
        }
        else if (randomValue <= 90.5f)  // 2단계: 하늘색, 15% 확률 (75.5% ~ 90.5%)
        {
            itemGrade = 2;
        }
        else if (randomValue <= 95.5f)  // 3단계: 보라색, 5% 확률 (90.5% ~ 95.5%)
        {
            itemGrade = 3;
        }
        else if (randomValue <= 98.5f)  // 4단계: 분홍색, 3% 확률 (95.5% ~ 98.5%)
        {
            itemGrade = 4;
        }
        else if (randomValue <= 99.5f)  // 5단계: 주황색, 1% 확률 (98.5% ~ 99.5%)
        {
            itemGrade = 5;
        }
        else if (randomValue <= 99.93f)  // 6단계: 노란색, 0.43% 확률 (99.5% ~ 99.93%)
        {
            itemGrade = 6;
        }
        else                          // 7단계: 빨간색, 0.07% 확률 (99.93% ~ 100%)
        {
            itemGrade = 7;
        }

        itemGrades[emptySlot] = itemGrade; // 선택된 등급을 슬롯에 저장

        // 슬롯에 레벨과 등급 UI 업데이트
        currentLevels[emptySlot] = 1; // 초기 레벨은 1로 설정
        slotOccupied[emptySlot] = true;
        enhanceButtons[emptySlot].interactable = true; // 구매 후 강화 버튼 활성화
        enhanceButtons[emptySlot].image.color = gradeColors[itemGrade - 1]; // 등급에 따른 버튼 색상 적용

        // 해당 아이템의 등급에 맞는 토글이 체크되어 있으면 즉시 판매
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
