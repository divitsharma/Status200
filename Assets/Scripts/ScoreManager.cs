﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event Action ScoreEvent;
    public event Action GameOverEvent;

    float score = 0f;
    public float Score { get => score; }
    bool gameOver = false;
    public bool GameOver { get => gameOver; }
    float GOTime;

    public static ScoreManager inst;


    // Start is called before the first frame update
    void Start()
    {
        GameOverEvent += OnGameOver;
        inst = this;
    }

    void OnDestroy()
    {
        GameOverEvent -= OnGameOver;
    }

    void Update()
    {
        if (!GameOver)
        {
            score += Time.deltaTime;
        }
    }
    void FixedUpdate()
    {
        if (GameOver)
        {
            Time.timeScale = Mathf.Lerp(1f, 0f, (Time.timeSinceLevelLoad - GOTime) / 2f);
        }
    }

    public void PlayerScored()
    {
        Debug.Log("scored!");
    }

    public void FailedTurn()
    {
        Debug.Log("failed turn");
        GameOverEvent?.Invoke();
    }

    void OnGameOver()
    {
        gameOver = true;
        GOTime = Time.timeSinceLevelLoad;
    }
}
