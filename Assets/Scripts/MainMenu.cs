﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public Player player;

    public Text scoreLabel;

    private void Awake()
    {
        Application.targetFrameRate = 1000;
        
        gameObject.SetActive(false);
    }

    public void StartGame(int mode)
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
    }

    public void EndGame(float distanceTraveled)
    {
        scoreLabel.text = ((int)(distanceTraveled * 10f)).ToString();
        gameObject.SetActive(true);
        Cursor.visible = true;
    }
}
