using System;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject newRowTimerText; // The text that displays the time left before a new row of boxes is spawned
    [SerializeField] private GameObject scoreText; // The text that displays the current score of the player
    [SerializeField] private GameObject currentElementText;
    [SerializeField] private GameObject nextElementText;

    private TextMeshProUGUI currentElementTextMesh;
    private TextMeshProUGUI nextElementTextMesh;

    void Awake(){
        currentElementTextMesh = currentElementText.GetComponent<TextMeshProUGUI>();
        nextElementTextMesh = nextElementText.GetComponent<TextMeshProUGUI>();
    }

    public void updateElementTexts(){
        Element.Type currentElement = GameManager.Instance.getPlayerElement();
        Element.Type nextElement = GameManager.Instance.getNextElement();

        currentElementTextMesh.text = "Current: <color=#" + getColor(currentElement) + ">" + currentElement.ToString() + "</color>";
        nextElementTextMesh.text = "Next: <color=#" + getColor(nextElement) + ">" + nextElement.ToString() + "</color>";
    }

    private String getColor(Element.Type element){
        switch (element){
            case Element.Type.FIRE:
                return "b13e53";
            case Element.Type.WATER:
                return "41a6f6";
            case Element.Type.GRASS:
                return "38b764";
            default:
                return "FFFFFF";
        }
    }

    public void updateScore(int newScore){
        // check if the scoreText is not null (if it is cast an error)
        if (scoreText == null){
            Debug.LogError("scoreText is not set in the HUD component");
            return;
        }

        // update the text of the scoreText
        scoreText.GetComponent<TextMeshProUGUI>().text = newScore.ToString();
    }

    public void updateNewRowTimer(float timeLeft){
        // check if the newRowTimerText is not null (if it is cast an error)
        if (newRowTimerText == null){
            Debug.LogError("newRowTimerText is not set in the HUD component");
            return;
        }

        // update the text of the newRowTimerText
        newRowTimerText.GetComponent<TextMeshProUGUI>().text = timeLeft.ToString("F2");
    }
}
