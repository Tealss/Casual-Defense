using UnityEngine;
using UnityEngine.UI;

public class GameSpeed : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button speedButton;
    [SerializeField] private Text speedText;

    private int currentSpeedIndex = 0;
    private readonly float[] speedLevels = { 1f, 2f, 10f };

    private void Start()
    {
        if (speedButton == null || speedText == null)
        {
            Debug.LogError("SpeedButton �Ǵ� SpeedText�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        speedButton.onClick.AddListener(OnSpeedButtonClick);

        UpdateGameSpeed();
    }

    private void OnSpeedButtonClick()
    {
        currentSpeedIndex = (currentSpeedIndex + 1) % speedLevels.Length;
        UpdateGameSpeed();
    }

    private void UpdateGameSpeed()
    {
        Time.timeScale = speedLevels[currentSpeedIndex];
        speedText.text = $"x {speedLevels[currentSpeedIndex]}";
    }
}
