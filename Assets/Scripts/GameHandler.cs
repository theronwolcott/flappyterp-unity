using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("GameHandler.Start");

        GameObject gameObject = new GameObject("Pipe", typeof(SpriteRenderer));

        Score.Start();
        // print(PlayerPrefs.GetInt("highscore"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
