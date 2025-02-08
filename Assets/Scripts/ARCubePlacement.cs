using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARCubePlacement : MonoBehaviour
{
    public GameObject cubePrefab; // Prefab of the cube to be placed
    public ARRaycastManager arRaycastManager; // Reference to ARRaycastManager for detecting planes
    public ARPlaneManager arPlaneManager; // Reference to ARPlaneManager for managing detected planes

    private ARImageCapture arImageCapture; // Reference to ARImageCapture for taking AR images
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>(); // List to store raycast results

    void Start()
    {
        // Find and assign the ARImageCapture component in the scene
        arImageCapture = FindObjectOfType<ARImageCapture>();
    }

    void Update()
    {
        // Check if the screen is touched and the touch phase is "Began"
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    void PlaceObject()
    {
        // Perform a raycast from the touch position to detect AR planes
        bool collision = arRaycastManager.Raycast(Input.GetTouch(0).position, raycastHits, TrackableType.PlaneWithinPolygon);

        if (collision)
        {
            // Get the first detected plane's position
            Vector3 spawnPosition = raycastHits[0].pose.position;

            // Adjust spawn position to place the cube above the plane
            spawnPosition.y += cubePrefab.transform.localScale.y / 2;

            // Instantiate the cube at the detected position
            GameObject _cubePrefab = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);

            // Assign a random color to the cube
            _cubePrefab.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);

            // Disable AR planes after placing the object
            DisablePlanes();

            // Capture an image after 10 frames
            arImageCapture.CaptureImageAfterFrames(10);
        }
    }

    void DisablePlanes()
    {
        // Disable all detected AR planes
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        // Disable AR plane detection
        arPlaneManager.enabled = false;
    }
}