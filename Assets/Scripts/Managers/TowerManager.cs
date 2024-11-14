using UnityEngine;

public class TowerManager : MonoBehaviour
{
    private GameObject currentBuildButton;
    private Canvas canvas;
    private ObjectPool objectPool;

    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        objectPool = FindObjectOfType<ObjectPool>();  // ObjectPool�� ������
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTileClick();
        }
    }

    private void HandleTileClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Tile"))
        {
            // Ÿ���� �߽� ��ġ�� ȭ����� ��ġ ���
            Vector3 tileCenter = hit.collider.transform.position;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(tileCenter) + new Vector3(0, 8f, 0);

            if (currentBuildButton == null)
            {
                // ��ư�� ������ Ǯ���� ������
                currentBuildButton = objectPool.GetFromPool(objectPool.towerBuildButtonPool);
                currentBuildButton.GetComponent<RectTransform>().SetParent(canvas.transform, false);
            }

            // ��ư�� ��ġ�� �׻� �ֽ� ȭ�� ��ġ�� ������Ʈ
            currentBuildButton.GetComponent<RectTransform>().position = screenPosition;
        }
        else if (currentBuildButton != null)
        {
            // Ÿ�� �� Ŭ�� ��, ��ư�� Ǯ�� ��ȯ
            objectPool.ReturnToPool(currentBuildButton, objectPool.towerBuildButtonPool);
            currentBuildButton = null;
        }
    }
}
