using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderToggle : MonoBehaviour
{
    [SerializeField]
    Animator myAnimator;

    [SerializeField]
    Animator myChildAnimator;

    bool isToggled;

    private void Start()
    {
        
        isToggled = false;
    }

    public void Toggle()
    {
        isToggled = !isToggled;

    
        Debug.Log(isToggled);

        if (isToggled)
        {
            myChildAnimator.SetTrigger("slideRight");
        }
        else
        {
            myChildAnimator.SetTrigger("slideLeft");
        }
    }
}
