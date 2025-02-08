using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARGameManager : MonoBehaviour
{
    public GameObject environment; // Reference to the AR environment
    public GameObject imageUI; // UI that displays the captured image

    void Start()
    {
        // Ensure the AR environment is active at the start
        environment.SetActive(true);

        // Hide the image UI initially
        imageUI.SetActive(false);
    }

    public void CaptureAgain()
    {
        // Reload the current scene to reset the AR experience
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
