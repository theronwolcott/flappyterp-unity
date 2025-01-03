using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;

    public static GameAssets GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }
    public Sprite pipeHeadSprite;
    public Transform PipeBodypf;
    public Transform PipeHeadpf;
    public Transform Groundpf;
    public Transform Cloud_1pf;
    public Transform Cloud_2pf;
    public Transform Cloud_3pf;

}
