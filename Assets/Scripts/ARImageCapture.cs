using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ARImageCapture : MonoBehaviour
{
    private Camera arCamera;
    private RenderTexture renderTexture;
    private Texture2D capturedImage;
    public RawImage displayImage; // UI RawImage to display captured image

    private string imagePath;

    void Start()
    {
        // Get the AR camera to capture the scene from the user's perspective
        arCamera = Camera.main;

        // Create a RenderTexture to capture the camera output
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);

        // Check if there is a saved image path from previous captures
        if (PlayerPrefs.HasKey("SavedImagePath"))
        {
            // Retrieve the saved image path from PlayerPrefs
            string savedPath = PlayerPrefs.GetString("SavedImagePath");

            // Check if the saved image exists and display it
            if (File.Exists(savedPath))
            {
                DisplayImageOnUI(savedPath);
            }
        }
    }

    // Initiates the image capture after a specified number of frames
    public void CaptureImageAfterFrames(int frameDelay)
    {
        StartCoroutine(CaptureImageCoroutine(frameDelay));
    }

    // Coroutine that waits for a delay before capturing the image
    private IEnumerator CaptureImageCoroutine(int frameDelay)
    {
        // Wait for the specified number of frames
        yield return new WaitForSeconds(frameDelay * Time.deltaTime);

        // Check if camera or render texture is null before proceeding
        if (arCamera == null || renderTexture == null)
        {
            Debug.LogError("Camera or RenderTexture is null!");
            yield break; // Exit the coroutine if there's an error
        }

        // Temporarily assign the RenderTexture to the AR camera for capturing the view
        RenderTexture previousTexture = arCamera.targetTexture;
        arCamera.targetTexture = renderTexture;

        // Wait for the frame to render fully
        yield return new WaitForEndOfFrame();

        // Capture the image from the render texture
        capturedImage = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        capturedImage.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        capturedImage.Apply();

        // Define the path to save the captured image in persistent storage
        imagePath = Application.persistentDataPath + "/AR_Capture.png"; // Use persistentDataPath for cross-platform saving
        File.WriteAllBytes(imagePath, capturedImage.EncodeToPNG()); // Save the captured image as PNG
        Debug.Log("Image Captured and saved at: " + imagePath);

        // Save the image path in PlayerPrefs so it can be reloaded later
        PlayerPrefs.SetString("SavedImagePath", imagePath);
        PlayerPrefs.Save(); // Ensure PlayerPrefs data is saved

        // Restore the original RenderTexture
        arCamera.targetTexture = previousTexture;
        RenderTexture.active = null;

        // Display the captured image on the UI
        DisplayImageOnUI(imagePath);
    }

    // Loads the captured image from file and displays it in the RawImage UI element
    private void DisplayImageOnUI(string path)
    {
        // Check if the image file exists at the specified path
        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path); // Read the image file into a byte array
            Texture2D loadedTexture = new Texture2D(2, 2); // Create a new Texture2D to load the image
            loadedTexture.LoadImage(imageBytes); // Load the image data into the texture

            // Check if the RawImage UI element is assigned, then display the image
            if (displayImage != null)
            {
                displayImage.texture = loadedTexture; // Set the loaded texture to the RawImage
                displayImage.enabled = true; // Enable the RawImage to display the texture
            }
            else
            {
                Debug.LogError("RawImage UI element not assigned.");
            }
        }
        else
        {
            // Log an error if the image file is not found at the specified path
            Debug.LogError("Image file not found at: " + path);
        }
    }
}
