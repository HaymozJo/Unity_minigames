using System;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class highScoreShown : MonoBehaviour
{
    public float addY = 25;
    public float startY = 275;
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
        Debug.Log("destroy state reached");
        while (transform.childCount > 0) {
            Debug.Log("yeeesir");
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("quit state reached");
        while (transform.childCount > 0) {
            Debug.Log("yeeesir");
            DestroyImmediate(transform.GetChild(0).gameObject);
        }       
    }

    public void setupScore()
    {

        for (int i = 1; i<=maxBestScores; i++)
        {
            GameObject score = Instantiate(scorePrefab, new Vector3(startX, startY-  i*addY , 0), transform.rotation);
            score.transform.SetParent(gameObject.transform);
            score.tag = scoreTag;
            score.gameObject.SetActive(true);
            GameObject name = Instantiate(scorePrefab, new Vector3(startX + addX, startY-  i*addY , 0), transform.rotation);
            name.transform.SetParent(gameObject.transform);
            name.tag = scoreTag;
            name.gameObject.SetActive(true);
        }
    }

}
