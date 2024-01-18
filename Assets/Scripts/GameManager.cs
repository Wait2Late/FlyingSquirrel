using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //public float mySpeedMultiplier = 1.0f;   // 1 för normal hastighet, 0.1 för Mårten Mode
    //public bool mySlowMode = false;
    public bool myInvertedControls = false;

    [SerializeField] PlayerController myPlayer;
    [SerializeField] DeathScreen myDeathScreen;
    [SerializeField] WinScreen myWinScreen;
    [SerializeField] GameObject[] myOrbs;
    [SerializeField] int myStartOrbs;
    [SerializeField] int myOrbsLeft;

    // In Game UI
    private bool myHasFound = false;
    //private GameObject myWinScreenObject;
    [SerializeField] Text myOrbCounter;
    [SerializeField] GameObject myTimer;
    [SerializeField] Text myFinalOrbCountText;
    [SerializeField] Text myCurrentTimeText;
    [SerializeField] Text myBestTimeText;

    // Level Select UI
    [SerializeField] GameObject myLevelInfo1;
    [SerializeField] GameObject myLevelInfo2;
    [SerializeField] GameObject myLevelInfo3;
    [SerializeField] Text myBestOrbsText1;
    [SerializeField] Text myBestOrbsText2;
    [SerializeField] Text myBestOrbsText3;
    [SerializeField] int[] myLevelAmountOfOrbs = new int[3];
    [SerializeField] Text myBestTimeText1;
    [SerializeField] Text myBestTimeText2;
    [SerializeField] Text myBestTimeText3;
        
    // In game timer and orb collecteds
    private StopWatch myTimerScript;
    public int[] myMaxCollectedOrbs = new int[3];

    [SerializeField]
    int myCurrentScore = 0;
    [SerializeField]
    int myHighScore = 0;

    public bool myLevelOneUnlocked = true;
    public bool myLevelTwoUnlocked = false;
    public bool myLevelThreeUnlocked = false;

    public static GameManager ourInstance { get; private set; }

    int mySceneNumber;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        myLevelAmountOfOrbs[0] = 16;
        myLevelAmountOfOrbs[1] = 17;
        myLevelAmountOfOrbs[2] = 38;

        //PlayerPrefs.SetFloat("Highscore 1", 0);
        //PlayerPrefs.SetFloat("Highscore 2", 0);
        //PlayerPrefs.SetFloat("Highscore 3", 0);
        //PlayerPrefs.SetInt("Max Orbs 1", 0);
        //PlayerPrefs.SetInt("Max Orbs 2", 0);
        //PlayerPrefs.SetInt("Max Orbs 3", 0);
        //PlayerPrefs.SetInt("Orbs 1", 0);
        //PlayerPrefs.SetInt("Orbs 2", 0);
        //PlayerPrefs.SetInt("Orbs 3", 0);

        if (PlayerPrefs.GetInt("Screenmanager Resolution Width") < 64)
        {
            PlayerPrefs.SetInt("Screenmanager Resolution Width", 1920);
            PlayerPrefs.SetInt("Screenmanager Resolution Height", 1080);
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        }

        Time.timeScale = 1;

        if (ourInstance == null)
        {
            ourInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        string[] temp = SceneManager.GetActiveScene().name.Split();

        for (int i = 0; i < temp.Length; i++)
        {
            bool success = int.TryParse(temp[i], out int index);

            if (success)
            {
                mySceneNumber = index;
                PlayerPrefs.SetInt("SceneNumber", mySceneNumber);
            }
        }
    }

    void OnSceneLoaded(Scene aScene, LoadSceneMode aLoadSceneMode)
    {
        if (FindObjectOfType<PlayerController>())
        {
            myHasFound = false;
            FindPlayerAndUI();
        }   
        else if (SceneManager.GetActiveScene().name == "LevelSelect")
        {
            FindLevelInfoUI();
        }
    }

    private void FindLevelInfoUI()
    {
        myLevelInfo1 = GameObject.Find("Level 1 Info");
        myLevelInfo2 = GameObject.Find("Level 2 Info");
        myLevelInfo3 = GameObject.Find("Level 3 Info");

        myBestOrbsText1 = GameObject.Find("Collected Orb Text 1").GetComponent<Text>();
        myBestOrbsText2 = GameObject.Find("Collected Orb Text 2").GetComponent<Text>(); ;
        myBestOrbsText3 = GameObject.Find("Collected Orb Text 3").GetComponent<Text>(); ;

        myBestTimeText1 = GameObject.Find("Best Time Text 1").GetComponent<Text>();
        myBestTimeText2 = GameObject.Find("Best Time Text 2").GetComponent<Text>();
        myBestTimeText3 = GameObject.Find("Best Time Text 3").GetComponent<Text>();

        UpdateLevelInfoUI();

        myLevelInfo1.SetActive(false);
        myLevelInfo2.SetActive(false);
        myLevelInfo3.SetActive(false);
    }
    private void UpdateLevelInfoUI()
    {
        myBestOrbsText1.text = string.Format("{0}/{1}",GetBestOrbCount(1), myLevelAmountOfOrbs[0]);
        myBestOrbsText2.text = string.Format("{0}/{1}",GetBestOrbCount(2), myLevelAmountOfOrbs[1]);
        myBestOrbsText3.text = string.Format("{0}/{1}",GetBestOrbCount(3), myLevelAmountOfOrbs[2]);

        myBestTimeText1.text = "BEST: " + TimeSpan.FromSeconds(PlayerPrefs.GetFloat("Highscore 1")).ToString("mm':'ss'.'ff");
        myBestTimeText2.text = "BEST: " + TimeSpan.FromSeconds(PlayerPrefs.GetFloat("Highscore 2")).ToString("mm':'ss'.'ff");
        myBestTimeText3.text = "BEST: " + TimeSpan.FromSeconds(PlayerPrefs.GetFloat("Highscore 3")).ToString("mm':'ss'.'ff");
    }

    public void FindPlayerAndUI()
    {
        if (!myHasFound)
        {
            myDeathScreen = GameObject.Find("UI").GetComponent<DeathScreen>();
            myWinScreen = GameObject.Find("UI").GetComponent<WinScreen>();

            myWinScreen.ShowToBeFound();

            myPlayer = GameObject.Find("PlayerPrefab").GetComponent<PlayerController>();
            myOrbCounter = GameObject.Find("Orb Counter").GetComponent<Text>();
            myFinalOrbCountText = GameObject.Find("Orb Count Text").GetComponent<Text>();
            myCurrentTimeText = GameObject.Find("Play time text").GetComponent<Text>();
            myBestTimeText = GameObject.Find("Best Time Text").GetComponent<Text>();
            myTimer = FindObjectOfType<StopWatch>().gameObject;
            myTimerScript = FindObjectOfType<StopWatch>();

            myDeathScreen.Hide();
            myWinScreen.Hide();

            myOrbs = GameObject.FindGameObjectsWithTag("Stamina Pickup");
            myStartOrbs = myOrbs.Length;
            GetOrbs();

            myHasFound = true;
        }
    }

    public void ToggleInvertedControls()
    {
        myInvertedControls = !myInvertedControls;
    }

    public bool GetMyInvertedcontrols()
    {
        return myInvertedControls;
    }

    public void SetMyInvertedControls(bool aState)
    {
        myInvertedControls = aState;
    }

    public GameObject GetPlayer()
    {
        return myPlayer.gameObject;
    }

    void Update()
    {
        if (FindObjectOfType<PlayerController>()) 
        {
            GetOrbs();
            int collectedOrbs = myStartOrbs - myOrbsLeft;

            if (GameObject.Find("PlayerPrefab") != null)
            {
                if (!myPlayer.myIsAlive)
                {
                    myDeathScreen.PlayerDied();
                }
                if (myPlayer.myIsInGoal && myPlayer.myIsAlive)
                {
                    myWinScreen.PlayerWon();

                    if (GetBestTimeFloat(PlayerPrefs.GetInt("SceneNumber")) == 0)
                    {
                        PlayerPrefs.SetFloat(("Highscore " + PlayerPrefs.GetInt("SceneNumber")), myTimerScript.myCurrentTime);
                    }
                    else
                    {
                        if (PlayerPrefs.GetFloat("Highscore " + PlayerPrefs.GetInt("SceneNumber")) > myTimerScript.myCurrentTime)
                        {
                            PlayerPrefs.SetFloat(("Highscore " + PlayerPrefs.GetInt("SceneNumber")), myTimerScript.myCurrentTime);
                        }
                    }

                    if (GetBestOrbCount(PlayerPrefs.GetInt("SceneNumber")) == 0)
                    {
                        PlayerPrefs.SetInt(("Orbs " + PlayerPrefs.GetInt("SceneNumber")), collectedOrbs);
                    }
                    else
                    {
                        if (PlayerPrefs.GetInt(("Orbs " + PlayerPrefs.GetInt("SceneNumber"))) < collectedOrbs)
                        {
                            PlayerPrefs.SetInt(("Orbs " + PlayerPrefs.GetInt("SceneNumber")), collectedOrbs);
                        }
                    }

                    myFinalOrbCountText.text = myOrbCounter.text;
                    myCurrentTimeText.text = "TIME: " + TimeSpan.FromSeconds(myTimerScript.myCurrentTime).ToString("mm':'ss'.'ff");
                    myBestTimeText.text = "BEST: " + TimeSpan.FromSeconds(GetBestTimeFloat(PlayerPrefs.GetInt("SceneNumber"))).ToString("mm':'ss'.'ff");
                }
            }
        }

        
    }

    public float GetBestTimeFloat(int index)
    {
        switch (index)
        {
            case 1:
                return PlayerPrefs.GetFloat("Highscore 1");
            case 2:
                return PlayerPrefs.GetFloat("Highscore 2");
            case 3:
                return PlayerPrefs.GetFloat("Highscore 3");
            default:
                return 0;
        }
    }

    public int GetBestOrbCount(int index)
    {
        switch (index)
        {
            case 1:
                return PlayerPrefs.GetInt("Orbs 1");
            case 2:
                return PlayerPrefs.GetInt("Orbs 2");
            case 3:
                return PlayerPrefs.GetInt("Orbs 3");
            default:
                return 0;
        }
    }

    public int GetMaxOrbCount(int index)
    {
        switch (index)
        {
            case 1:
                return PlayerPrefs.GetInt("Max Orbs 1");
            case 2:
                return PlayerPrefs.GetInt("Max Orbs 2");
            case 3:
                return PlayerPrefs.GetInt("Max Orbs 3");
            default:
                return 0;
        }
    }

    private void GetOrbs()
    {
        myOrbs = GameObject.FindGameObjectsWithTag("Stamina Pickup");
        PlayerPrefs.SetInt("Max Orbs " + mySceneNumber, myOrbs.Length);
        myOrbsLeft = myOrbs.Length;
        myOrbCounter.text = string.Format("{0}/{1}", myStartOrbs - myOrbsLeft, myStartOrbs);
    }

    public void RespawnPlayer()
    {
        myPlayer.myIsAlive = true;
    }
}
