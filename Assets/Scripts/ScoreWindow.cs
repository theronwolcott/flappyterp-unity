using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreWindow : MonoBehaviour
{
    // private Text ScoreText;
    private TextMeshProUGUI ScoreText;
    private TextMeshProUGUI HighScoreText;
    // private int test;

    private void Awake()
    {
        ScoreText = transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        HighScoreText = transform.Find("HighScoreText").GetComponent<TextMeshProUGUI>();

    }

    private void Start() {
        HighScoreText.text = "HIGHSCORE: " + Score.GetHighScore().ToString();
    }

    private void Update()
    {
        ScoreText.text = Level.GetInstance().GetPipesPassed().ToString();
    }
}
