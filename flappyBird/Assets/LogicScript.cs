using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System;
using TMPro;

public class LogicScript : MonoBehaviour
{

    public int playerScore;
    public Text scoreText;
    public Text nameText;
    public Text highScoreText;
    public GameObject gameOverScreen;
    public GameObject highScoreScreen;
    public GameObject nameScreen;
    public TMP_InputField inputField;
    public GameObject menuScreen;

    private bool alreadyPlayed = false;
    private bool running = false;
    private int highScore;
    private string playerName;

    private int maxBestScore = 5;

    [ContextMenu("Increase Score")]
    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
    }

    public void SetName()
    {
        PlayerPrefs.SetString("LastName", inputField.text);
    }

    public void ApplyName()
    {
        nameText.text = PlayerPrefs.GetString("LastName");
    }


    public void SetHighScore()
    {
        playerName = PlayerPrefs.GetString("LastName").ToString();
        string highscoreStr = "HS" + playerName;

        alreadyPlayed = PlayerPrefs.HasKey(highscoreStr);
        if (alreadyPlayed){
            highScore = PlayerPrefs.GetInt(highscoreStr);
            if (playerScore > highScore){
                PlayerPrefs.SetInt(highscoreStr, playerScore);
                highScoreText.text = playerScore.ToString();
            }else{
                highScoreText.text = highScore.ToString();
            }
        }else{
            PlayerPrefs.SetInt(highscoreStr, 0);
            highScoreText.text = "0";
        }
    }

        public void UpdateHighScores()
    {
        //get the player currently playing
        string playerName = PlayerPrefs.GetString("LastName");

        for (int i=1; i<= maxBestScore; i++)
        {   
            
            string highscoreStr = "HS" + playerName;
            int highscore = PlayerPrefs.GetInt(highscoreStr);
            string key = i.ToString() + "HS";
            bool scoreExist = PlayerPrefs.HasKey(key);
            if (scoreExist){
                int scoreToBeat = PlayerPrefs.GetInt(key);
                string nameKey = i+"HSname";
                string otherPlayerName = PlayerPrefs.GetString(nameKey);
                if (playerName==otherPlayerName){
                    if (highscore>=scoreToBeat){
                         PlayerPrefs.SetInt(key, highscore);
                    }
                    break;
                }else{
                    if (highscore>=scoreToBeat){
                        PlayerPrefs.SetInt(key, highscore);
                        PlayerPrefs.SetString(nameKey, playerName);
                        //Now we have to get down the new number by now using their score and name
                        playerName = otherPlayerName ;
                    }
                }
                
            }else{
                PlayerPrefs.SetInt(key, highscore);
                string nameKey = i.ToString() +"HSname";
                PlayerPrefs.SetString(nameKey, playerName);
                break;
            }
                //Check if  a best score exist//if best score exist, check if bigger, 
        }

        for (int i=1; i<= maxBestScore; i++){
            string key = i.ToString() + "HS";
            string nameKey = i.ToString() +"HSname";
            int score = PlayerPrefs.GetInt(key, 0);
            string name = PlayerPrefs.GetString(nameKey, "Unknown");
            Debug.Log(name + " this guy has the best score of: "+ score.ToString()+ "placed; "+ i.ToString());
        }

    }


    public void restartGame()
    {
        SceneManager.LoadSceneAsync(1);
        running = true;
    }
    
    public void mainMenu()
    {
        if (running){
            UpdateHighScores();
            running = false;
        }
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
