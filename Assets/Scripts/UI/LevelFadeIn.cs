using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelFadeIn : MonoBehaviour
{
    [SerializeField] Image myFadeInImage;

    float myFadeInTime = 2.5f;

    float myStartFadeTimer = 0.5f;

    private void Start()
    {
        Color currentColor = myFadeInImage.color;
        currentColor.a = 1;
        myFadeInImage.color = currentColor;
    }
    private void Update()
    {
        Color currentColor = myFadeInImage.color;

        myStartFadeTimer -= Time.deltaTime;

        if (myStartFadeTimer <= 0)
        {
            myStartFadeTimer = 0;

            if (currentColor.a > 0)
            {
                currentColor.a -= (1 / myFadeInTime) * Time.deltaTime;
                myFadeInImage.color = currentColor;
            }
        }
    }
}
