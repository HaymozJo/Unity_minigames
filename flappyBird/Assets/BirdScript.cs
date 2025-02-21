using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public LogicScript logic;
    public bool birdAlive;
    public float flapStrength;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        birdAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && birdAlive)
        {
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
        }
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        logic.gameOver();
        birdAlive = false;
    }
}
