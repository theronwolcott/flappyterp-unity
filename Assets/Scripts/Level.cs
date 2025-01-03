using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private const float PIPE_WIDTH = 7.8f;
    private const float PIPE_HEAD_HEIGHT = 3.75f;
    private const float PIPE_MOVE_SPEED = 30f;
    private const float PIPE_DESTROY_X = -100f;
    private const float PIPE_SPAWN_X = 100f;
    private const float GROUNDE_DESTROY_X = -200f;
    private const float GROUND_SPAWN_X = 100f;
    private const float CLOUD_DESTROY_X = -150f;
    private const float CLOUD_SPAWN_X = 150f;
    float pipeHeadY;
    float pipeBodyY;

    private static Level instance;

    public static Level GetInstance()
    {
        return instance;
    }

    private List<Transform> groundList;
    private List<Transform> cloudList;
    private float cloudTimer;
    private List<Pipe> pipes;
    private List<Pipe> pipes2;
    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private int pipesSpawned;
    private float pipesPassed;
    private State state;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }

    private enum State
    {
        Idle,
        Playing,
        Dead,
    }

    // level is generated
    private void Awake()
    {
        instance = this;
        pipes = new List<Pipe>();
        pipes2 = new List<Pipe>();
        SpawnGround();
        SpawnClouds();
        pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
        state = State.Idle;
    }

    private void Start()
    {
        Bird.GetInstance().OnDeath += Bird_OnDeath;
        Bird.GetInstance().OnStart += Bird_OnStart;
        // Score.ResetHighScore();
    }

    private void Bird_OnStart(object sender, System.EventArgs e)
    {
        state = State.Playing;
    }

    private void ResetScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    private void Bird_OnDeath(object sender, System.EventArgs e)
    {
        Debug.Log("Hit!");
        state = State.Dead;
        // delay the scene reset by one second
        //Invoke("ResetScene", 1);
    }

    private void Update()
    {
        if (state == State.Playing)
        {
            HandlePipeMovement();
            HandlePipeSpawning();
            HandleGround();
            HandleClouds();
        }
    }

    private void SpawnClouds() {
        Transform  cloudTransform;
        cloudList = new List<Transform>();
        float cloudY = 30f;
        cloudTransform = Instantiate(RandomCloud(), new Vector3(0, cloudY, 0), Quaternion.identity);
        cloudList.Add(cloudTransform);
    }

    private Transform RandomCloud() {
        switch (Random.Range(0, 3)) {
            default:
            case 0: return GameAssets.GetInstance().Cloud_1pf;
            case 1: return GameAssets.GetInstance().Cloud_2pf;
            case 2: return GameAssets.GetInstance().Cloud_3pf;
        }
    }

    private void HandleClouds() {
        cloudTimer -= Time.deltaTime;
        if (cloudTimer < 0) {
            // spawn new cloud
            cloudTimer = 6f;
            float cloudY = 30f;
            Transform cloudTransform = Instantiate(RandomCloud(), new Vector3(CLOUD_SPAWN_X, cloudY, 0), Quaternion.identity);
            cloudList.Add(cloudTransform);
        }
        // cloud moving
        for (int i = 0; i < cloudList.Count; i++) {
            Transform cloudTransform = cloudList[i];
            cloudTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime * 0.7f;
            if (cloudTransform.position.x < CLOUD_DESTROY_X) {
                Destroy(cloudTransform.gameObject);
                cloudList.RemoveAt(i);
                i--;
            }
        }
    }

    private void SpawnGround() {
        Transform groundTransform;
        groundList = new List<Transform>();
        float groundY = -47.9f;
        float groundWidth = 165f;
        groundTransform = Instantiate(GameAssets.GetInstance().Groundpf, new Vector3(0, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().Groundpf, new Vector3(groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform = Instantiate(GameAssets.GetInstance().Groundpf, new Vector3(groundWidth * 2, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
    }

    private void HandleGround() {
        foreach (Transform t in groundList) {
            t.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            if (t.position.x < GROUNDE_DESTROY_X) {
                float rightPos = -100f;
                for (int i = 0; i < groundList.Count; i++) {
                    if (groundList[i].position.x > rightPos) {
                        rightPos = groundList[i].position.x;
                    }
                }
                // place ground on right most position
                float groundWidth = 165f;
                t.position = new Vector3(rightPos + groundWidth, t.position.y, t.position.z);
            }
        }
    }

    private void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;
        if (pipeSpawnTimer < 0)
        {
            // spawn another pipe
            pipeSpawnTimer += pipeSpawnTimerMax;

            /* minimum height to spawn must be half the gap size
            so the gap reaches its full height, then we  add 10
            so there aren't gaps with no pipe showing */
            float minHeight = gapSize / 2 + 10f;

            /* max height is the reverse, we start at the top,
            subtract half the gap size so it reaches full height, 
            and then subtract another 10 so they don't spawn
            all the way at the top  */
            float maxHeight = 100 - (gapSize / 2) - 10;
            float height = Random.Range(minHeight, maxHeight);
            CreateGaps(height, gapSize, PIPE_SPAWN_X);
        }
    }

    private void HandlePipeMovement()
    {
        for (int i = 0; i < pipes.Count; i++)
        {
            // move pipe across the screen
            Pipe pipe = pipes[i];
            pipe.Move();
            // destroy pipes after they pass by
            if (pipe.GetX() < PIPE_DESTROY_X)
            {
                pipes.Remove(pipe);
                pipe.DestroyPipe();
                // need to iterate same index after removing
                i--;
            }
        }
        // need to update score as we pass a pipe
        for (int j = 0; j < pipes2.Count; j++)
        {
            Pipe checkLeft = pipes2[j];
            // if pipe passes bird
            if (checkLeft.GetX() < 0f)
            {
                pipes2.Remove(checkLeft);
                /* technically, there are two pipes being accounted for as we pass through
                each gap, so we increase by 0.5 for each so our score will increase by 1 */
                pipesPassed += 0.5f;
                j--;
            }
        }
    }

    private void SetDifficulty(Difficulty diff)
    {
        if (diff == Difficulty.Easy)
        {
            gapSize = 50f;
            pipeSpawnTimerMax = 1.2f;
        }
        else if (diff == Difficulty.Medium)
        {
            gapSize = 41f;
            pipeSpawnTimerMax = 1.1f;
        }
        else if (diff == Difficulty.Hard)
        {
            gapSize = 33f;
            pipeSpawnTimerMax = 1.0f;
        }
        else
        {
            gapSize = 24f;
            pipeSpawnTimerMax = 0.9f;
        }
    }

    private Difficulty GetDifficulty()
    {
        if (pipesSpawned >= 30)
        {
            return Difficulty.Impossible;
        }
        else if (pipesSpawned >= 20)
        {
            return Difficulty.Hard;
        }
        else if (pipesSpawned >= 10)
        {
            return Difficulty.Medium;
        }
        else
        {
            return Difficulty.Easy;
        }
    }

    private void CreateGaps(float y, float size, float xpos)
    {
        // bottom pipe
        CreatePipe(y - (size / 2f), xpos, true);
        // top pipe
        CreatePipe(100 - y - (size / 2), xpos, false);
        // need to track amount of pipes spawn so we can update difficulty
        pipesSpawned++;
        // update difficulty
        SetDifficulty(GetDifficulty());
    }

    private void CreatePipe(float height, float xpos, bool bottom)
    {

        // make head at x and top of body, height/2 makes sure head doesn't stick above body 
        Transform pipeHead = Instantiate(GameAssets.GetInstance().PipeHeadpf);

        if (bottom)
        {
            pipeHeadY = height - (PIPE_HEAD_HEIGHT / 2f) - 50;
        }
        else
        {
            pipeHeadY = (PIPE_HEAD_HEIGHT / 2f) - height + 50;
        }
        pipeHead.position = new Vector3(xpos, pipeHeadY);

        // make body at x and bottom of screen (-50 is bottom)
        Transform pipeBody = Instantiate(GameAssets.GetInstance().PipeBodypf);

        if (bottom)
        {
            pipeBodyY = -50;
        }
        else
        {
            pipeBodyY = 50;
            pipeBody.localScale = new Vector3(1, -1, 1);
        }
        pipeBody.position = new Vector3(xpos, pipeBodyY);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyCollider.offset = new Vector2(0f, (height / 2f));

        // make a Pipe object and add to list
        Pipe pipe = new Pipe(pipeHead, pipeBody);
        pipes.Add(pipe);
        pipes2.Add(pipe);
    }

    public int GetPipesSpawned()
    {
        return pipesSpawned;
    }

    public float GetPipesPassed()
    {
        return pipesPassed;
    }

    private class Pipe
    {

        private Transform headTrans;
        private Transform bodyTrans;

        public Pipe(Transform headTrans, Transform bodyTrans)
        {
            this.headTrans = headTrans;
            this.bodyTrans = bodyTrans;
        }

        public void Move()
        {
            headTrans.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            bodyTrans.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetX()
        {
            return headTrans.position.x;
        }

        public void DestroyPipe()
        {
            Destroy(headTrans.gameObject);
            Destroy(bodyTrans.gameObject);
        }
    }
}
