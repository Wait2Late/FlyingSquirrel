using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject myDeathScreen;
    [SerializeField] float myTimeToShow = 0.5f;
    bool myIsPaused;

    public void PlayerDied()
    {
        Show();
    }

    public void Hide()
    {
        myDeathScreen.SetActive(false);
        myIsPaused = false;
        Time.timeScale = 1;
    }

    void Show()
    {
        myTimeToShow -= Time.deltaTime;
        if (myTimeToShow <= 0)
        {
            myDeathScreen.SetActive(true);
            myIsPaused = true;
        }
    }
}
