using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARCubePlacement : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;  // Prefab for the cube to be instantiated
    [SerializeField] private GameObject imageUI;     // UI element to be shown after placement

    private ARRaycastManager arRaycastManager;  // Manages AR raycasting for touch-based placement
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // Stores raycast hit results
    private Camera arCamera; // Reference to the AR Camera
    private ARImageCapture arImageCapture; // Manages AR image capture functionality
    private ARGameManager arGameManager; // Manages game state and UI elements

    void Start()
    {
        // Finds AR Raycast Manager in the scene, responsible for detecting AR surfaces
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        // Gets reference to the main AR Camera
        arCamera = Camera.main;

        // Finds the AR Image Capture script in the scene, ensuring image capture functionality
        arImageCapture = FindObjectOfType<ARImageCapture>();

        // Finds the AR Game Manager script, which handles UI state changes
        arGameManager = FindObjectOfType<ARGameManager>();
    }

    void Update()
    {
        // Checks if the user has touched the screen and if the touch just began
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceCube();
        }
    }

    void PlaceCube()
    {
        // Generates a random position within a predefined range
        float randomX = Random.Range(-3f, 3f);  // Random X position between -3 and 3
        float randomZ = Random.Range(-3f, 3f);  // Random Z position between -3 and 3
        float randomY = 0.5f;  // Fixed Y position to keep cubes above ground

        Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

        // Instantiates the cube at the generated position with default rotation
        GameObject cube = Instantiate(cubePrefab, randomPosition, Quaternion.identity);

        // Assigns a random color to the instantiated cube
        cube.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);

        // Captures an image after 10 frames using ARImageCapture
        arImageCapture.CaptureImageAfterFrames(10);

        // Initiates a coroutine to enable imageUI after a 3-second delay
        StartCoroutine(ShowImageUIAfterDelay(3f));
    }

    IEnumerator ShowImageUIAfterDelay(float delay)
    {
        // Waits for the specified duration before executing the next line
        yield return new WaitForSeconds(delay);

        // Disables the environment object to focus on UI display
        arGameManager.environment.SetActive(false);

        // Activates the UI element after the delay
        arGameManager.imageUI.SetActive(true);
    }
}