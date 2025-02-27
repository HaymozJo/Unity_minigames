using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public int playerScore;
    public Text scoreText;
    public GameObject gameOverScreen;

    [ContextMenu("Increase Score")]
    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
    }

    public void restartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    
    public void mainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void quitGame()
    {
        Application.Quit();
    }
    public void gameOver()
    {
        gameOverScreen.SetActive(true);
    }
}
