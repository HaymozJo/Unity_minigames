using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class highScoreShown : MonoBehaviour
{
    public float addY = 22;
    public float startY = 235;
    public float startX = 275; 
    public float addX = 100;
    public GameObject scorePrefab;

    private int maxBestScores = 5;
    private string scoreTag = "HS";
    private bool execute = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Update()
    {
        if (execute){
            setupScore();
            execute = false;
        }
    }

    void OnDestroy()
    {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    void OnApplicationQuit()
    {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }       
    }

    public void setupScore()
    {
        
        for (int i = 1; i<=maxBestScores; i++)
        {
            string nameStr = PlayerPrefs.GetString(i.ToString()+ "HSname");
            string scoreStr = PlayerPrefs.GetInt(i.ToString() + "HS").ToString();

            GameObject score = Instantiate(scorePrefab, new Vector3(startX + addX, startY-  i*addY , 0), transform.rotation);
            GameObject name = Instantiate(scorePrefab, new Vector3(startX, startY-  i*addY , 0), transform.rotation);
           
            score.transform.SetParent(gameObject.transform);
            score.tag = scoreTag;
            Text scoreText = score.GetComponent<Text>();
            scoreText.text = scoreStr;
            score.gameObject.SetActive(true);

            name.transform.SetParent(gameObject.transform);
            name.tag = scoreTag;
            Text nameText = name.GetComponent<Text>();
            nameText.text = nameStr;
            name.gameObject.SetActive(true);

        }
    }

}
