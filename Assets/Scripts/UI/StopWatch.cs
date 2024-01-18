using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class StopWatch : MonoBehaviour
{
    public float myCurrentTime = 0;

    [SerializeField] private Text myTextBox;
    private TimeSpan myTimePlaying;
    PlayerController myPlayer;
    private string[] myLevelNames = { "Level_1", "Level_2", "Level_3" };

    private void Awake()
    {
        myTextBox = GameObject.Find("TextBox").GetComponent<Text>();
    }

    void Start()
    {
        myTextBox.text = "Time: 00:00.00";
    }

    void Update()
    {
        myPlayer = GameObject.Find("PlayerPrefab").GetComponent<PlayerController>();
        if (!myPlayer.myIsInGoal && myPlayer.myIsAlive)
        {
            myCurrentTime += Time.deltaTime;
        }

        myTimePlaying = TimeSpan.FromSeconds(myCurrentTime);
        string timePlayingStr = "Time: " + myTimePlaying.ToString("mm':'ss'.'ff") + '\t';
        myTextBox.text = timePlayingStr; //Visar i det högra hörnet tiden som spelas ut
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}

