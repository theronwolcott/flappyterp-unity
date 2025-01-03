using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private const float JUMP_FORCE = 100f;

    public event EventHandler OnDeath;
    public event EventHandler OnStart;
    public static Bird instance;

    public static Bird GetInstance() {
        return instance;
    }
    private Rigidbody2D birdBody;
    private State state;

    public enum State {
        Idle,
        Playing,
        Dead,
    }

    private void Awake() {
        instance = this;
        birdBody = gameObject.GetComponent<Rigidbody2D>();
        birdBody.bodyType = RigidbodyType2D.Static;
        state = State.Idle;
    }

    // Update is called once per frame
    void Update() {
        if (state == State.Idle) {
            // if we are idle and click
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
                state = State.Playing;
                birdBody.bodyType = RigidbodyType2D.Dynamic;
                Jump();
                if (OnStart != null) {
                    OnStart(this, EventArgs.Empty);
                }
            }
        } else if (state == State.Playing) {
            // if we are currently playing
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
                Jump();
            }
        }   
    }

    private void Jump() {
        birdBody.velocity = Vector2.up * JUMP_FORCE;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        birdBody.bodyType = RigidbodyType2D.Static;
        if (OnDeath != null) {
            OnDeath(this, EventArgs.Empty);
            //GameOverWindow.Show(GameOverWindow.GetGameOverWindow());
        }
    }

    /*public static bool GetIsDead(Bird bird) {
        return bird.isDead;
    }*/
}
