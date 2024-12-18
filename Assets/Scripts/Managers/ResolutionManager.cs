using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    void Start()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        //Debug.Log($"Screen Resolution: {screenWidth}x{screenHeight}");

        int targetWidth = 1920;
        int targetHeight = 1080;
        float aspectRatio = (float)screenWidth / screenHeight;

        if (aspectRatio > (float)targetWidth / targetHeight)
        {
            Screen.SetResolution(targetWidth, (int)(targetWidth / aspectRatio), true);
        }
        else
        {
            Screen.SetResolution((int)(targetHeight * aspectRatio), targetHeight, true);
        }
    }
}