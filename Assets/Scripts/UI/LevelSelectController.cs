using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour
{
    GameManager myGameManager;

    [SerializeField] GameObject myLevelOneButton;
    [SerializeField] GameObject myLevelTwoButton;
    [SerializeField] GameObject myLevelThreeButton;

    [SerializeField] Image myLightImage;

    [SerializeField] float myTransitionTime = 3;

    bool myHasClicked = false;
    


    private void Awake()
    {
        myGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public float GetTransitionTime()
    {
        return myTransitionTime;
    }

    private void Start()
    {
        if (!myGameManager.myLevelOneUnlocked)
        {
            myLevelOneButton.SetActive(false);
        }
        if (!myGameManager.myLevelTwoUnlocked)
        {
            myLevelTwoButton.SetActive(false);
        }
        if (!myGameManager.myLevelThreeUnlocked)
        {
            myLevelThreeButton.SetActive(false);
        }
    }



    private void Update()
    {
        if (myHasClicked)
        {
            Color currentColor = myLightImage.color;
            currentColor.a+= (1/myTransitionTime)*Time.deltaTime;
            myLightImage.color = currentColor;
        }
    }

    public void DisableButtons()
    {
        myLevelOneButton.SetActive(false);
          
        myLevelTwoButton.SetActive(false);
       
        myLevelThreeButton.SetActive(false);
    }    

    public void StartIncreasingLight()
    {
        myHasClicked = true;
    }
}
