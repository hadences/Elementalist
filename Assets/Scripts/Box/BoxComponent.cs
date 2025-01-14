using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BoxComponent : MonoBehaviour
{
    public UnityEvent<int> onBoxDamaged;

    [Header("Box Settings")]
    [SerializeField] private Vector2 minMaxHealth = new Vector2(1, 3);
    private float health = 1;
    
    [Header("References")]
    [SerializeField] private TextMeshPro healthText;
    [SerializeField] private GameObject fireSprite;
    [SerializeField] private GameObject waterSprite;
    [SerializeField] private GameObject grassSprite;

    private Dictionary<Element.Type, GameObject> elementSprites = new Dictionary<Element.Type, GameObject>();

    private Element.Type boxElement = Element.Type.FIRE;

    private BoxSpawner spawner;

    void Start(){
        elementSprites.Add(Element.Type.FIRE, fireSprite);
        elementSprites.Add(Element.Type.WATER, waterSprite);
        elementSprites.Add(Element.Type.GRASS, grassSprite);

        // set the health of the box
        health = (int) Random.Range(minMaxHealth.x, minMaxHealth.y);

        // select a random element for the box
        boxElement = (Element.Type)Random.Range(0, 3);

        // hide all the element sprites
        foreach(KeyValuePair<Element.Type, GameObject> entry in elementSprites){
            entry.Value.SetActive(false);
        }

        // show the element sprite of the box
        elementSprites[boxElement].SetActive(true);

        // update the health text
        updateHealthText();
    }

    /* hurt the ball whenever the ball makes contact with it */
    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Ball")){
            int damageValue = 1; // default damage value
            int score = GameManager.Instance.getScorePerBoxHit();

            // get the element of the player
            Element.Type playerElement = GameManager.Instance.getPlayerElement();
    
            // check if the player element is strong against the box element
            if(Element.getStrength(playerElement) == boxElement){
                damageValue *= 2; // double the damage
                score *= 2; // double the score
            }else if(Element.getWeakness(playerElement) == boxElement){
                damageValue = 0; // half the damage
                score = 0; // half the score
            }
            
            health -= damageValue;

            onBoxDamaged.Invoke(score);

            updateHealthText();
            
            if(health <= 0){
                destroyBox();
            }   
        }
    }

    private void updateHealthText(){
        healthText.text = health.ToString();
    }

    public void setSpawner(BoxSpawner spawner){
        this.spawner = spawner;
    }

    public void destroyBox(){
        spawner.getBoxes().Remove(gameObject);
        Destroy(gameObject);
    }
}
