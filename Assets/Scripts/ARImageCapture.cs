using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ARImageCapture : MonoBehaviour
{
    public RawImage displayImage; // UI element to display the captured image

    private Camera arCamera; // Reference to the AR camera
    private Texture2D capturedImage; // Stores the captured image
    private string imagePath; // Path to save the image
    private RenderTexture renderTexture; // RenderTexture for capturing the screen

    private ARGameManager arGameManager; // Reference to the AR game manager

    void Start()
    {
        arCamera = Camera.main;
        arGameManager = FindObjectOfType<ARGameManager>();

        // Initialize renderTexture once instead of creating it every time
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
    }

    public void CaptureImageAfterFrames(int frameDelay)
    {
        StartCoroutine(CaptureImageCoroutine(frameDelay));
    }

    private IEnumerator CaptureImageCoroutine(int frameDelay)
    {
        yield return new WaitForSeconds(frameDelay * Time.deltaTime);

        // Capture camera view into a texture
        arCamera.targetTexture = renderTexture;
        yield return new WaitForEndOfFrame();

        capturedImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        capturedImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        capturedImage.Apply();

        // Save the image as a PNG file
        imagePath = Application.persistentDataPath + "/AR_Capture.png";
        File.WriteAllBytes(imagePath, capturedImage.EncodeToPNG());

        // Cleanup
        arCamera.targetTexture = null;
        RenderTexture.active = null;

        // Display the captured image on UI
        DisplayImage(imagePath);

        // Disable AR environment and show the captured image UI
        arGameManager.environment.SetActive(false);
        arGameManager.imageUI.SetActive(true);
        displayImage.gameObject.SetActive(true);
    }

    private void DisplayImage(string path)
    {
        if (!File.Exists(path)) return;

        // Load and assign the captured image to the UI
        Texture2D loadedTexture = new Texture2D(2, 2);
        loadedTexture.LoadImage(File.ReadAllBytes(path));
        displayImage.texture = loadedTexture;
    }
}
