//using UnityEngine;
//using UnityEngine.UI;
//using SFB; // StandaloneFileBrowser ���ӽ����̽�
//using System.IO;

//public class ProfileImageManager : MonoBehaviour
//{
//    [Header("UI Components")]
//    public Button changeProfileButton;
//    public Image profileImage;

//    private void Start()
//    {
//        changeProfileButton.onClick.AddListener(OnChangeProfileImage);
//    }

//    private void OnChangeProfileImage()
//    {
//        var paths = StandaloneFileBrowser.OpenFilePanel("������ �̹��� ����", "", "png,jpg,jpeg", false);

//        if (paths.Length == 0 || string.IsNullOrEmpty(paths[0])) return;

//        string filePath = paths[0];

//        byte[] imageData = File.ReadAllBytes(filePath);
//        Texture2D texture = new Texture2D(2, 2);
//        texture.LoadImage(imageData);

//        Sprite newProfileSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
//        profileImage.sprite = newProfileSprite;

//        Debug.Log($"������ �̹��� ���� �Ϸ�: {filePath}");
//    }
//}
