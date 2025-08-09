using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlockSmasherMinigameManagerScript : MonoBehaviour
{
    public static BlockSmasherMinigameManagerScript Instance { get; private set; }
    public BallScript ball { get; private set; }
    public PaddleScript paddle { get; private set; }
    public BrickScript[] bricks { get; private set; }

    public int level = 1;
    public int score = 0;
    public int scoreGoal { get; private set; } = 0;
    public TextMeshProUGUI scoreText;
    public int lives = 4;
    public TextMeshProUGUI livesText;

    private float backButtonDownTime = 0f;
    public float holdThreshold = 1.0f; // seconds

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        this.score = 0;
        this.lives = 1;
        UpdateUI();

        LoadLevel(1);

    }

    private void Update()
    {
        // Detect back button press (Escape on Android)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            backButtonDownTime = Time.time;
        }

        // Detect back button release
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            float heldDuration = Time.time - backButtonDownTime;

            if (heldDuration >= holdThreshold)
            {
                Debug.Log("Back button held!");
                OnBackButtonHeld();
            }
            else
            {
                Debug.Log("Back button tapped!");
                OnBackButtonTapped();
            }
        }
    }

    private void OnBackButtonHeld()
    {
        SceneManager.LoadScene("LevelSelectScreen");
    }

    private void OnBackButtonTapped()
    {
        Miss();
    }

    /// <summary>
    /// Two functions used to communicate with GameCycleController
    /// </summary>
    /// <param name="goal"></param>
    public void SetScoreGoal(int goal)
    {
        scoreGoal = goal;
    }

    public bool IsGameComplete()
    {
        return score >= scoreGoal;
    }

    private void LoadLevel(int level)
    {
        this.level = level;

        if (level > 5)
        {
            // Restart game, keep score the same.
            this.level = 1;
            SceneManager.LoadScene("Level" + level);
        }
        else
        {
            SceneManager.LoadScene("Level" + level);
        }
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        this.ball = FindObjectOfType<BallScript>();
        this.paddle = FindObjectOfType<PaddleScript>();
        this.bricks = FindObjectsOfType<BrickScript>();

        FindAndAssignUIReferences();
        UpdateUI();
    }

    private void ResetLevel()
    {
        this.ball.ResetBall();
        this.paddle.ResetPaddle();

    }

    private void GameOver()
    {
        //SceneManager.LoadScene("GameOverScreen");

        NewGame();
    }


    public void Hit(BrickScript brick)
    {
        this.score += brick.points;
        UpdateUI();

        if (Cleared())
        {
            LoadLevel(this.level + 1);
        }
    }

    public void Miss()
    {
        this.lives--;
        UpdateUI();

        if (lives > 0)
        {
            ResetLevel();
        }
        else
        {
            GameOver();
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < this.bricks.Length; i++)
        {
            if (this.bricks[i].isActiveAndEnabled && !this.bricks[i].unbreakable)
            {
                return false;
            }
        }

        return true;
    }

    public void UpdateUI()
    {
        if (scoreText != null)
            this.scoreText.text = "Score: " + this.score;

        if (livesText != null)
            this.livesText.text = "Lives: " + this.lives;
    }

    public void FindAndAssignUIReferences()
    {
        // Look for objects named "ScoreText" and "LivesText" in the new scene
        GameObject scoreObj = GameObject.Find("ScoreText");
        GameObject livesObj = GameObject.Find("LivesText");

        if (scoreObj != null)
            scoreText = scoreObj.GetComponent<TextMeshProUGUI>();

        if (livesObj != null)
            livesText = livesObj.GetComponent<TextMeshProUGUI>();
    }

}
