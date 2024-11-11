using UnityEngine;

public class SkyRotate : MonoBehaviour
{
    public float rotationSpeed = 1f; // ȸ�� �ӵ�

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}