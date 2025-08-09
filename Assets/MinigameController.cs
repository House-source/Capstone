using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public static GameSceneController instance;

    public int CurrentScore { get; private set; }
    public bool IsGameComplete { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
        if (CurrentScore >= 400)
        {
            IsGameComplete = true;
        }
    }

    // Call this when game ends
    public void CompleteGame()
    {
        IsGameComplete = true;
    }
}
