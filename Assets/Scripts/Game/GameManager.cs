using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("Other References")]
    [SerializeField] private Animator screenAnimator;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject highScoreText;

    [Header("Screen References")]
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject hudScreen;
    [SerializeField] private GameObject howToPlayScreen;

    [Header("Game References")]
    [SerializeField] private GameObject game;
    [SerializeField] private GameObject playerSpawnpoint;
    [SerializeField] private GameObject ballSpawnpoint;
    [SerializeField] private GameObject lostPoint; // the point where if a box passes it the game ends
    [SerializeField] private BoxSpawner boxSpawner;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject ballPrefab;

    [Header("Game Settings")]
    [SerializeField] private int scorePerBoxHit = 10; // the score value you get when you hit/damage a box
    [SerializeField] private int newBoxRowTime = 10; // the number of seconds before a new row of boxes is spawned

    private InputAction changeElementAction;

    private float timer; // The current countdown 

    private Element.Type playerElement = Element.Type.FIRE; // the element that the player currently has

    private GameObject player;
    private GameObject ball;

    private int currentScore = 0;
    private int highScore = 0;


    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadHighScore();

        changeElementAction = InputSystem.actions.FindAction("Jump");
        if(changeElementAction == null){
            Debug.LogError("Change Element action not found");
            Application.Quit();
        }

        if(!changeElementAction.enabled){
            changeElementAction.Enable();
        }

        changeElementAction.performed += ctx => traverseElement();

        showMainScreen();
    }

    // save the high score before the game is closed
    void OnApplicationQuit(){
        saveHighScore();
    }

    void FixedUpdate()
    {
        checkIfGameEnded();        
    }

    private void checkIfGameEnded() {
        List<GameObject> boxes = boxSpawner.getBoxes();
        foreach (GameObject box in boxes){
            if (box != null){
                if (box.transform.position.y < lostPoint.transform.position.y){
                    endGame();
                    return;
                }
            }
        }
    }

    private void saveHighScore(){
        PlayerPrefs.SetInt("highScore", highScore);
    }

    private void loadHighScore(){
        highScore = PlayerPrefs.GetInt("highScore", 0);
    }

    public void startGame(){

        // clear existing boxes
        boxSpawner.clearBoxes();

        // delete current palyer and ball
        if (player != null){
            Destroy(player);
        }

        if (ball != null){
            Destroy(ball);
        }

        // reset the score
        currentScore = 0;

        HUD hud = hudScreen.GetComponent<HUD>();
        hud.updateElementTexts();
        hud.updateScore(currentScore);

        // unpause the game
        Time.timeScale = 1; 

        // spawn the player
        player = Instantiate(playerPrefab, playerSpawnpoint.transform.position, Quaternion.identity, game.transform);

        // spawn the ball
        ball = Instantiate(ballPrefab, ballSpawnpoint.transform.position, Quaternion.identity, game.transform);

        showHudScreen();

        // start the countdown
        timer = newBoxRowTime;
        StartCoroutine(TimerCountdown());

        // spawn the first row of boxes
        boxSpawner.spawnBoxes();
    }

    public void endGame(){
        SoundManager.Instance.playSound(SoundManager.gameOverSound);
        // pause the game
        StopAllCoroutines();
        Time.timeScale = 0;

        // update the score text
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + currentScore;

        // update the high score
        if (currentScore > highScore){
            highScore = currentScore;
        }
        highScoreText.GetComponent<TextMeshProUGUI>().text = "Best: " + highScore;

        showEndScreen();
    }

    public void exitGame(){
        // Exit the game
        Application.Quit();
    }

    private void showMainScreen()
    {
        howToPlayScreen.SetActive(false);
        mainScreen.SetActive(true);
        endScreen.SetActive(false);
        hudScreen.SetActive(false);
        
        game.SetActive(false);
    }

    private void showEndScreen()
    {
        mainScreen.SetActive(false);
        endScreen.SetActive(true);
        hudScreen.SetActive(false);
    }

    private void showHudScreen()
    {
        howToPlayScreen.SetActive(false);
        mainScreen.SetActive(false);
        endScreen.SetActive(false);
        hudScreen.SetActive(true);

        game.SetActive(true);
    }

    public int getScorePerBoxHit(){
        return scorePerBoxHit;
    }

    /*
     * Triggers whenever a box is hit by the ball
     * Increments the score by the scorePerBoxHit value
     */
    public void onBoxDamaged(int newScore){
        currentScore += newScore;
        hudScreen.GetComponent<HUD>().updateScore(currentScore);
    }

    /*
     * Countdown timer for spawning a new row of boxes
     */
    private IEnumerator TimerCountdown(){
        while(true){
            timer = newBoxRowTime;

            while(timer > 0){
                timer -= Time.deltaTime;

                // update the timer in the HUD
                hudScreen.GetComponent<HUD>().updateNewRowTimer(timer);

                yield return null;
            }

            // TODO: spawn a new row of boxes
            // Debug.Log("Spawn a new row of boxes");
            boxSpawner.spawnNewRow();
        }
    }

    /*
     * Gets the player element
     * typically used to determine the damage value for a box whenever the ball hits it
     */
    public Element.Type getPlayerElement(){
        return playerElement;
    }

    /*
     * traverses through the next element whenever the player presses the change element button (space)
     */
    private void traverseElement(){
        switch(playerElement){
            case Element.Type.FIRE:
                playerElement = Element.Type.WATER;
                break;
            case Element.Type.WATER:
                playerElement = Element.Type.GRASS;
                break;
            case Element.Type.GRASS:
                playerElement = Element.Type.FIRE;
                break;
            default:
                playerElement = Element.Type.FIRE;
                break;
        }

        HUD hud = hudScreen.GetComponent<HUD>();
        hud.updateElementTexts();
        ball.GetComponent<BallComponent>().updateBallElement();
    }

    public Element.Type getNextElement(){
        switch(playerElement){
            case Element.Type.FIRE:
                return Element.Type.WATER;
            case Element.Type.WATER:
                return Element.Type.GRASS;
            case Element.Type.GRASS:
                return Element.Type.FIRE;
            default:
                return Element.Type.FIRE;
        }
    }

    public void onPressHowToPlay(){
        // show the how to play screen

        howToPlayScreen.SetActive(true);
        screenAnimator.Play("FadeIn");
    }

    public void onPressOutHowToPlay(){
        // hide the how to play screen

        screenAnimator.Play("FadeOut");
    }

    public void onFadeOutComplete(){
        howToPlayScreen.SetActive(false);
    }
}
