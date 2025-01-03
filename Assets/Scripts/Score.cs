using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    public static void Start() {
        Bird.GetInstance().OnDeath += Bird_OnDeath;
    }

    private static void Bird_OnDeath(object sender, System.EventArgs e) {
        TryNewHighScore((int)Level.GetInstance().GetPipesPassed());
    }

    public static int GetHighScore() {
        return PlayerPrefs.GetInt("highscore");
    }

    public static bool TryNewHighScore(int score) {
        int currHighScore = GetHighScore();
        if (score > currHighScore) {
            // new high score
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public static void ResetHighScore() {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.Save();
    }
}
