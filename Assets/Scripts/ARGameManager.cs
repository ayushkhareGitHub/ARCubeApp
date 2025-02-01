using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARGameManager : MonoBehaviour
{
    public GameObject environment; // Reference to the environment object in the scene
    public GameObject imageUI; // Reference to the UI that appears after an action

    void Start()
    {
        // Ensures the environment is visible when the scene starts
        environment.SetActive(true);

        // Hides the image UI at the beginning of the scene
        imageUI.SetActive(false);
    }

    public void CaptureAgain()
    {
        // To capture image again, we are reloading the current active scene to reset the AR experience
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
