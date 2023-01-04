using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameOverMenu : MonoBehaviour
{
    TextMeshProUGUI currentScoreUI;
    TextMeshProUGUI highScoreUI;
    // Start is called before the first frame update
    void Start()
    {
        currentScoreUI = GameObject.Find("CurrentScore").GetComponent<TextMeshProUGUI>();
        highScoreUI = GameObject.Find("HighScore").GetComponent<TextMeshProUGUI>();
        currentScoreUI.text = "Score: " + GameManager.score;
        highScoreUI.text = "High Score: " + GameManager.highestScore;
    }
    public void GoToPlay() {
        HealthBar.Reset();
        SceneManager.LoadScene("Play");
    }
    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
