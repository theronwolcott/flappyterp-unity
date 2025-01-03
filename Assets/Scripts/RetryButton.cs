using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    public void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void LoadMainMenuScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
