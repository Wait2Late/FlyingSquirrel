using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionMenu : MonoBehaviour
{
    [SerializeField] Dropdown myDropdown;
    Toggle myToggle;
    bool myFullScreen;
    List<Resolution> myResolutions;
    Vector2 myCurrentResolution;

    void Awake()
    {
        myFullScreen = Screen.fullScreen;
        myDropdown = GameObject.Find("Resolution Dropdown").GetComponent<Dropdown>();
        myResolutions = new List<Resolution>();
        myDropdown.ClearOptions();

        Resolution[] allResolutions = Screen.resolutions;

        foreach (Resolution res in allResolutions)
        {
            if ((float)res.width / res.height >= 1.7777f && (float)res.width / res.height < 1.77777777f)
            {
                myResolutions.Add(res);
            }
        }


        if (myResolutions.Count > 4)
        {
            for (int i = 0; i < 5; i++)
            {
                myResolutions.RemoveAt(0);
            }
            for (int i = 0; i < 5; i++)
            {
                myResolutions.RemoveAt(1);
            }
            for (int i = 0; i < 5; i++)
            {
                myResolutions.RemoveAt(2);
            }
            for (int i = 0; i < 5; i++)
            {
                myResolutions.RemoveAt(3);
            }
        }

        List<string> options = new List<string>();

        int currentResIndex = 0;

        if (PlayerPrefs.GetInt("Resolution x") == 0)
        {
            for (int i = 0; i < myResolutions.Count; i++)
            {
                if (myResolutions[i].width == Screen.currentResolution.width && myResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIndex = i;
                }
            }
        }
        else
        {
            for (int i = 0; i < myResolutions.Count; i++)
            {
                if (myResolutions[i].width == PlayerPrefs.GetInt("Resolution x") && myResolutions[i].height == PlayerPrefs.GetInt("Resolution y"))
                {
                    currentResIndex = i;
                }
            }
        }

        foreach (Resolution res in myResolutions)
        {
            if ((float)res.width / res.height >= 1.7777f && (float)res.width / res.height < 1.77777777f)
            {
                string option = res.width + " x " + res.height;
                options.Add(option);
            }
        }

        myDropdown.AddOptions(options);
        myDropdown.value = currentResIndex;
        myDropdown.RefreshShownValue();

        if (PlayerPrefs.GetInt("Resolution x") == 0)
        {
            myCurrentResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }

        if (myFullScreen != myToggle.isOn)
        {
            myToggle.isOn = myFullScreen;
        }
    }

    public void SetResolution(int aResolutionIndex)
    {
        Resolution res = myResolutions[aResolutionIndex];
        myToggle = GameObject.Find("Fullscreen Toggle").GetComponent<Toggle>();
        bool fullscreen = myToggle.isOn;

        if (myFullScreen == myToggle.isOn)
        {
            Screen.SetResolution(res.width, res.height, myFullScreen);
        }
        else
        {
            Screen.SetResolution(res.width, res.height, !myFullScreen);
        }

        PlayerPrefs.SetInt("Resolution x", res.width);
        PlayerPrefs.SetInt("Resolution y", res.height);

        myCurrentResolution = new Vector2(res.width, res.height);
    }

    public void SetFullScreen(bool aToggleState)
    {
        myToggle = GameObject.Find("Fullscreen Toggle").GetComponent<Toggle>();

        Screen.SetResolution((int)myCurrentResolution.x, (int)myCurrentResolution.y, aToggleState);

        if (myFullScreen == myToggle.isOn)
        {
            Screen.SetResolution((int)myCurrentResolution.x, (int)myCurrentResolution.y, myFullScreen);
        }
        else
        {
            Screen.SetResolution((int)myCurrentResolution.x, (int)myCurrentResolution.y, !myFullScreen);
        }

        myFullScreen = aToggleState;
    }

    void Update()
    {

    }
}
