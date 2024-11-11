using UnityEngine;

public class UIRotate : MonoBehaviour
{
    public float rotationSpeed = 100f;

    private void Start()
    {
        
    }
    void Update()
    {
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
}