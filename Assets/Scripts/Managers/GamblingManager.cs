using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamblingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button gambleButton;
    public Transform[] slotParents;
    public GameObject[] slotPrefabs;
    public Image[] winLines;
    public GameObject spinEffectPrefab;
    public GameObject prizeObject;
    public Text prizeText;

    [Header("Timings")]
    public float slotSwitchInterval = 0.25f;
    public float spinDuration = 3f;

    private List<int[]> slotResults = new List<int[]>();
    private bool isSpinning = false;

    private const int MIN_MATCH_COUNT = 3;

    void Start()
    {
        gambleButton.onClick.AddListener(StartGamble);
        SetSlotsActive(false);
    }

    private void StartGamble()
    {
        if (isSpinning) return;

        if (GameManager.I.ticket <= 0)
        {
            ShowGoldDeductionFeedback();
            return;
        }

        GameManager.I.ticket--;

        isSpinning = true;
        gambleButton.interactable = false;
        slotResults.Clear();

        ResetSlots();

        SetSlotsActive(true);
        StartCoroutine(SpinAllSlots());

        //SoundManager.I.PlaySoundEffect(16);
    }

    IEnumerator SpinAllSlots()
    {
        for (int i = 0; i < slotParents.Length; i++)
        {
            SoundManager.I.PlaySoundEffect(17);
            StartCoroutine(SpinSingleSlot(slotParents[i], i));
            yield return new WaitForSeconds(0.4f);
        }
        SoundManager.I.PlaySoundEffect(17);
    }

    IEnumerator SpinSingleSlot(Transform slotParent, int slotIndex)
    {
        float elapsed = 0f;
        int randomIndex = 0;
        GameObject currentSlotObject = null;

        while (elapsed < spinDuration)
        {
            if (currentSlotObject != null) Destroy(currentSlotObject);

            randomIndex = Random.Range(0, slotPrefabs.Length);
            currentSlotObject = Instantiate(slotPrefabs[randomIndex], slotParent);
            elapsed += slotSwitchInterval;

            yield return new WaitForSeconds(slotSwitchInterval);
        }

        slotResults.Add(new int[] { randomIndex });
        CreateSpinEffect(slotParent);
        SoundManager.I.PlaySoundEffect(15);

        if (slotResults.Count == slotParents.Length)
        {
            EvaluateResults();
            isSpinning = false;
        }
    }

    private void CreateSpinEffect(Transform parent)
    {
        if (spinEffectPrefab != null)
        {
            var effect = Instantiate(spinEffectPrefab, parent);
            effect.transform.SetAsLastSibling();
        }
    }

    private void EvaluateResults()
    {
        Dictionary<int, List<int>> prefabSlotIndexes = CountPrefabSlotIndexes();

        //bool hasWinningSlots = false;

        foreach (var pair in prefabSlotIndexes)
        {
            if (pair.Value.Count >= MIN_MATCH_COUNT)
            {
                HighlightSlots(pair.Value);
                int reward = CalculateReward(pair.Value.Count);

                prizeObject.SetActive(true);
                prizeText.text = ($"+ {reward}");
                GameManager.I.AddGold(reward);
                //hasWinningSlots = true;
            }
        }

        //if (!hasWinningSlots)
        //{
        //    Debug.Log("Faild.");
        //}

        StartCoroutine(ResetAfterDelay());
    }
    private Dictionary<int, List<int>> CountPrefabSlotIndexes()
    {
        Dictionary<int, List<int>> prefabSlotIndexes = new Dictionary<int, List<int>>();

        for (int i = 0; i < slotResults.Count; i++)
        {
            int prefabIndex = slotResults[i][0];

            if (!prefabSlotIndexes.ContainsKey(prefabIndex))
            {
                prefabSlotIndexes[prefabIndex] = new List<int>();
            }

            prefabSlotIndexes[prefabIndex].Add(i);
        }

        return prefabSlotIndexes;
    }


    private int CalculateReward(int matchedCount)
    {
        switch (matchedCount)
        {
            case 3: return 3000;
            case 4: return 5000;
            case 5: return 10000;
            default: return 0;
        }
    }
    private void HighlightSlots(List<int> slotIndexes)
    {

        foreach (int slotIndex in slotIndexes)
        {
            Transform slotParent = slotParents[slotIndex];
            if (slotParent.childCount > 0)
            {
                var slotObject = slotParent.GetChild(0).GetComponent<Image>();
                if (slotObject != null)
                {
                    slotObject.color = Color.red;
                }
            }
        }
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        ResetSlots();
        ResetSlotColors();
        SetSlotsActive(false);
        prizeObject.SetActive(false);
        gambleButton.interactable = true;
    }

    private void ResetSlots()
    {
        foreach (Transform slotParent in slotParents)
        {
            foreach (Transform child in slotParent)
            {
                if (child != null && IsSlotInstance(child.gameObject))
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    private void ResetSlotColors()
    {
        foreach (Transform slotParent in slotParents)
        {
            if (slotParent.childCount > 0)
            {
                var slotObject = slotParent.GetChild(0).GetComponent<Image>();
                if (slotObject != null)
                {
                    slotObject.color = Color.white; // √ ±‚»≠ (»Úªˆ)
                }
            }
        }
    }

    private bool IsSlotInstance(GameObject obj)
    {
        foreach (var prefab in slotPrefabs)
        {
            if (obj.name.Contains(prefab.name)) return true;
        }
        return spinEffectPrefab != null && obj.name.Contains(spinEffectPrefab.name);
    }

    private void SetSlotsActive(bool isActive)
    {
        foreach (var slotParent in slotParents)
        {
            slotParent.gameObject.SetActive(isActive);
        }
    }

    private void ShowGoldDeductionFeedback()
    {
        RectTransform buttonRectTransform = gambleButton.GetComponent<RectTransform>();
        Vector3 buttonPosition = buttonRectTransform.anchoredPosition;
        FadeOutTextUse.I.SpawnFadeOutText(buttonPosition, "Not enough tickets", Color.red, true);

    }

}
