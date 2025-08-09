using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameCycleController : MonoBehaviour
{
    public string[] gameScenes; // Set in Inspector: list of scene names
    public GameObject infoPanel; // UI panel to show messages
    public Text infoText; // Text component for messages
    public float displayDuration = 3f; // seconds to display message
    public int startingScoreGoal = 400;
    public int scoreIncrement = 400;

    private int currentGameIndex = 0;
    private int currentScoreGoal;

    public BlockSmasherMinigameManagerScript bsManagerScript;
    

    void Start()
    {
        // Initialize variables for the first game
        currentScoreGoal = startingScoreGoal;
        bsManagerScript = FindObjectOfType<BlockSmasherMinigameManagerScript>();

        // Check if the necessary components are assigned in the Inspector.
        if (gameScenes == null || gameScenes.Length == 0 || infoPanel == null || infoText == null)
        {
            Debug.LogError("Missing required components in the Inspector!");
            return; // Exit if not initialized properly
        }

        LoadNextGame();
    }

    private void Update()
    {
        if (bsManagerScript != null)
        {

            // Check if game is completed
            if (bsManagerScript.IsGameComplete())
            {
                LoadNextGame();
            }
        }
    }

    void LoadNextGame()
    {
        if (currentGameIndex == 0)
        {
            StartCoroutine(LoadAndDisplayMessage());
        }
        else
        {
            // If there are no more game scenes, loop back to the beginning
            if (currentGameIndex >= gameScenes.Length)
            {
                currentGameIndex = 0;
            }

            // Update the score goal for the next game
            currentScoreGoal += scoreIncrement;

            StartCoroutine(LoadAndDisplayMessage());
        }
    }


    IEnumerator LoadAndDisplayMessage()
    {
        // Display a message in the info panel.
        infoPanel.SetActive(true);
        infoText.text = $"Reach {currentScoreGoal} points!";


        yield return new WaitForSeconds(displayDuration);

        // Hide the info panel
        infoPanel.SetActive(false);


        //Load the next scene
        SceneManager.LoadScene(gameScenes[currentGameIndex]);
        currentGameIndex++;
    }


    
}