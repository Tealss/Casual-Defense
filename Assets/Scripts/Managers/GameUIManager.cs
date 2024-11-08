using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUiManager : MonoBehaviour
{
    [Header("�˾�â")]
    public GameObject[] popupWindows;
    public Button[] toggleButtons;

    [Header("�ؽ�Ʈ")]
    public Text goldText;

    public static GameUiManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i;
            toggleButtons[i].onClick.AddListener(() => TogglePopup(index));
        }

        // ��� UI �ʱ�ȭ �� ������Ʈ �ڷ�ƾ ����
        UpdateGoldUI(GameManager.Instance.Gold);
        StartCoroutine(UpdateGoldCoroutine());
    }

    // ��� UI ���� �޼���
    public void UpdateGoldUI(int gold)
    {
        goldText.text = $" : {gold}";
    }

    // ��� UI�� �� �ʸ��� �����ϴ� �ڷ�ƾ
    private IEnumerator UpdateGoldCoroutine()
    {
        while (true)
        {
            // GameManager�� ��� ���� �����ͼ� UI ������Ʈ
            UpdateGoldUI(GameManager.Instance.Gold);

            // 1�� ���
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void TogglePopup(int index)
    {
        if (index >= 0 && index < popupWindows.Length)
        {
            bool isActive = popupWindows[index].activeSelf;
            popupWindows[index].SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("��ȿ���� ���� �˾� â �ε���");
        }
    }
}
