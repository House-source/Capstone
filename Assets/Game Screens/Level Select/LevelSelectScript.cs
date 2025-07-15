using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("TitleScreen"); // Returns to Title Screen
        }
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // Loads chosen Level
    }

}
