using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LeaderboardData leaderboard;
    public TMP_Text tempText;

    void Start()
    {
        tempText.text = "Best: " + leaderboard.best.ToString("#0.00");
    }

    public void OnPlay()
    {
        SceneManager.LoadScene(1);
    }
}
