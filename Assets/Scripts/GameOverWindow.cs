using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameOverWindow : MonoBehaviour
{
    private TextMeshProUGUI ScoreText;
    private TextMeshProUGUI HighScoreText;
    public static GameOverWindow instance;
    //private Transform transform;
    //private Bird bird = Bird.GetInstance();
    //public event EventHandler Bird_OnDeath;

    public static GameOverWindow GetGameOverWindow()
    {
        return instance;
    }
    private void Awake()
    {
        Debug.Log("awake");
        ScoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        HighScoreText = transform.Find("HighScoreText").GetComponent<TextMeshProUGUI>();
        // var button = transform.Find("RetryButton");
        //Hide();
        // transform.Find("RetryButton").GetComponent<Button>();
    }
    private void Start()
    {
        Bird.GetInstance().OnDeath += Bird_OnDeath;
        Hide();
    }

    /*private void Update() {
        Debug.Log("testupdate");
        if (Bird.GetIsDead(Bird.GetInstance()) == true) {
            Show();
        }
    }*/
    private void Bird_OnDeath(object sender, System.EventArgs e)
    {
        Debug.Log("testdeath");
        ScoreText.text = Level.GetInstance().GetPipesPassed().ToString();
        if (Level.GetInstance().GetPipesPassed() > Score.GetHighScore()) {
            HighScoreText.text = "NEW HIGHSCORE";
        } else {
            HighScoreText.text = "HIGHSCORE: " + Score.GetHighScore();
        }
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        //GetComponent<MeshRenderer>().enabled = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        //GetComponent<MeshRenderer>().enabled = true;
    }

    // public void Restart()
    // {
    //     UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    // }
}
