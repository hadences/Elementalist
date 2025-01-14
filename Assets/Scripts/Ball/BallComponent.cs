using System.Collections.Generic;
using UnityEngine;

public class BallComponent : MonoBehaviour
{
    [Header("Ball Settings")]
    [SerializeField] public float initialSpeed = 5.0f; 

    [Header("References")]
    [SerializeField] private GameObject fireSprite;
    [SerializeField] private GameObject waterSprite;
    [SerializeField] private GameObject grassSprite;

    private Dictionary<Element.Type, GameObject> elementSprites = new Dictionary<Element.Type, GameObject>();

    private Rigidbody2D rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        elementSprites.Add(Element.Type.FIRE, fireSprite);
        elementSprites.Add(Element.Type.WATER, waterSprite);
        elementSprites.Add(Element.Type.GRASS, grassSprite);

        updateBallElement();

        launchBall();
    }

    public void updateBallElement(){
        Element.Type currentElement = GameManager.Instance.getPlayerElement();

        foreach(KeyValuePair<Element.Type, GameObject> entry in elementSprites){
            entry.Value.SetActive(false);
        }

        elementSprites[currentElement].SetActive(true);
    }
    
    void Update(){
        correctBallVelocity();
    }

    private void launchBall(){
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = 1;

        Vector2 initialDirection = new Vector2(x, y);
        rb.linearVelocity = initialDirection * initialSpeed;
    }

    private void correctBallVelocity(){
        Vector2 ballVelocity = rb.linearVelocity;
        
        if(Mathf.Abs(ballVelocity.y) < 0.5){
            ballVelocity.y = Mathf.Sign(ballVelocity.y) * 0.5f;
        }

        if(Mathf.Abs(ballVelocity.x) < 0.5){
            ballVelocity.x = Mathf.Sign(ballVelocity.x) * 0.5f;
        }

        rb.linearVelocity = ballVelocity.normalized * initialSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
            GameManager.Instance.endGame();
        }

        SoundManager.Instance.playSound(SoundManager.hitSound, 0.5f, 1.0f);
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, rb.linearVelocity.normalized);
    }
}
