using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System;
using TMPro;

public class LogicScript : MonoBehaviour
{

    public static LogicScript Instance;
    public int playerScore;
    public Text scoreText;
    public Text nameText;
    public GameObject gameOverScreen;
    public GameObject highScoreScreen;
    public GameObject nameScreen;
    public TMP_InputField inputField;
    public GameObject menuScreen;


    [ContextMenu("Increase Score")]
    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
    }

    public void SetName()
    {
        MainManager.Instance.name = inputField.text;
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

    public void CreateName()
    {
        menuScreen.SetActive(false);
        nameScreen.SetActive(true);
    } 

    public void HighScoreOn()
    {
        menuScreen.SetActive(false);
        highScoreScreen.SetActive(true);
    }
    public void HighScoreOff()
    {
        menuScreen.SetActive(true);
        highScoreScreen.SetActive(false);
    }
    public void gameOver()
    {
        gameOverScreen.SetActive(true);
    }
}
