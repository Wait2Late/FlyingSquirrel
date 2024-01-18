using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FlightModeToggleScript : MonoBehaviour
{
    GameManager myGm;
    Toggle myToggle;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myGm = GameObject.Find("GameManager").GetComponent<GameManager>();
        myToggle = GetComponent<Toggle>();

        if (myGm.GetMyInvertedcontrols() != myToggle.isOn)
        {
            myToggle.isOn = !myToggle.isOn;
            myGm.ToggleInvertedControls();
        }
        

    }

    public void ToggleInvertedMode()
    {
        myGm = GameObject.Find("GameManager").GetComponent<GameManager>();
        myGm.ToggleInvertedControls();
    }
}
