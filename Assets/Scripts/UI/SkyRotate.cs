using UnityEngine;

public class SkyRotate : MonoBehaviour
{
    public float rotationSpeed = 1f; // 회전 속도

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}