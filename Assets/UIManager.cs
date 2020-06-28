using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TMP_Text runningScoreLabel;
    public GameObject GOPanel;

    void Start()
    {
        ScoreManager.inst.GameOverEvent += OnGameOver;
        GOPanel.SetActive(false);
    }

    void OnDestroy()
    {
        Debug.Log("destroy");
        ScoreManager.inst.GameOverEvent -= OnGameOver;
    }

    void Update()
    {
        runningScoreLabel.text = ScoreManager.inst.Score.ToString("#0.00");
    }

    void OnGameOver()
    {
        GOPanel.SetActive(true);
        TMP_Text scorelabel = GOPanel.transform.GetComponentsInChildren<TMP_Text>()[1];
        scorelabel.text = "> Score: " + ScoreManager.inst.Score.ToString("#0.00");
        runningScoreLabel.gameObject.SetActive(false);
    }

    public void OnRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
