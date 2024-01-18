using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private GameObject myWinScreen;
    private bool myIsPaused;

    public void PlayerWon()
    {
        Show();
    }
    public void Hide()
    {
        myWinScreen.SetActive(false);
        myIsPaused = false;
        Time.timeScale = 1;
    }

    public void ShowToBeFound()
    {
        myWinScreen.SetActive(true);
    }

    void Show()
    {
        GameObject.Find("GameManager").GetComponent<PreloadScript>().Load("LevelSelect");
        myWinScreen.SetActive(true);
        myIsPaused = true;
        Time.timeScale = 0;
    }
}
