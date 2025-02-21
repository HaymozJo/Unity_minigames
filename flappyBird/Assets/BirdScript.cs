using System.Collections;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public Sprite staticBird;
    public Sprite jumpBird;
    public Sprite deadBird;
    public Rigidbody2D myRigidbody;
    public LogicScript logic;
    public bool birdAlive;
    public float flapStrength;

    public float degree = 280;

    private AudioManager audioManager;
    private SpriteRenderer sr;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent <SpriteRenderer> ();
        sr.sprite = staticBird;
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        birdAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && birdAlive)
        {
            audioManager.PlaySFX(audioManager.Jump);
            myRigidbody.linearVelocity = Vector2.up * flapStrength;
            StartCoroutine (ChangeFace (jumpBird));
        }
        if ((transform.position.y > 5.87) || (transform.position.y < -5.06)){
            KillBird();
        }

        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        KillBird();
    }

    private void KillBird(){
        audioManager.PlaySFX(audioManager.Col);
        birdAlive = false;
        transform.Rotate(new Vector3(0, 0, 35), Space.Self);
        GetComponent<SpriteRenderer>().sprite = deadBird;
        logic.gameOver();
    }

    public IEnumerator ChangeFace (Sprite changeToSprite)
    {
    sr.sprite = changeToSprite;
    yield return new WaitForSeconds (0.1f);
    sr.sprite = staticBird;
    }

}