using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager instance;

    public GameObject Start_Page;
    public GameObject GameOver_Page;
    public GameObject CountDown_Page;
    public Text Score_Text;

    enum PageState
    {
        start,
        gameover,
        countdown,
        none
    }

    int score = 0;
    bool gameover = false;

    public bool gameOver { get { return gameover; } }

    private void Awake()
    {
        instance = this;
    }

    void OnCountDownFinished()
    {
        setPageState(PageState.none);
        OnGameStarted();
        score = 0;
        gameover = false;
    }

    void OnPlayerScored()
    {
        score++;
        Score_Text.text = score.ToString();
    }

    void OnPlayerDied()
    {
        gameover = true;
        int SavedScore = PlayerPrefs.GetInt("HighScore");
        if (score>SavedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        setPageState(PageState.gameover);
    }

    void OnEnable()
    {
        CountdownText.OnCountDownFinished += OnCountDownFinished;
        TapController.OnPlayerScored += OnPlayerScored;
        TapController.OnPlayerDied += OnPlayerDied;
    }

    void OnDisable()
    {
        CountdownText.OnCountDownFinished -= OnCountDownFinished;
        TapController.OnPlayerScored -= OnPlayerScored;
        TapController.OnPlayerDied -= OnPlayerDied;
    }

    void setPageState(PageState state)
    {
        switch (state)
        {
            case PageState.start:

                Start_Page.SetActive(true);
                GameOver_Page.SetActive(false);
                CountDown_Page.SetActive(false);
                break;

            case PageState.gameover:

                Start_Page.SetActive(false);
                GameOver_Page.SetActive(true);
                CountDown_Page.SetActive(false);
                break;

            case PageState.countdown:

                Start_Page.SetActive(false);
                GameOver_Page.SetActive(false);
                CountDown_Page.SetActive(true);
                break;

            case PageState.none:

                Start_Page.SetActive(false);
                GameOver_Page.SetActive(false);
                CountDown_Page.SetActive(false);
                break;
        }
    }

    public void ConfirmGameOVer()
    {
        OnGameOverConfirmed();
        Score_Text.text = "0";
        setPageState(PageState.start);
    }

    public void StartGame()
    {
        setPageState(PageState.countdown);
    }
}
