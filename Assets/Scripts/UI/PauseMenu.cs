using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject myPauseMenu;
    [SerializeField] private GameObject myDeathScreen;
    [SerializeField] private GameObject myWinScreen;
    bool myIsPaused;

    private void Awake()
    {
        myIsPaused = false;
        myPauseMenu.SetActive(false);
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && !myDeathScreen.activeSelf && !myWinScreen.activeSelf)
        {
            if (myIsPaused == true)
            {
                Hide();
            }
            else
            {
                Show();               
            }
        }
    }

    void Hide()
    {
        myPauseMenu.SetActive(false);
        myIsPaused = false;
        Time.timeScale = 1;
    }

    void Show()
    {
        myPauseMenu.SetActive(true);
        myIsPaused = true;
        Time.timeScale = 0;
    }
}
