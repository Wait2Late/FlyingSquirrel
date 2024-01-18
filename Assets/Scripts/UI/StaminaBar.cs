using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image myStaminaImage;

    float myMaxStamina = 100.0f;
    public float myMinStamina = 0.0f;
    public float myCurrentStamina = 100.0f;

    public float myStaminaDrain = 8.33f;
    public float myStaminaDrainAlways = 1f;

    private PlayerController myPlayerController;

    // Start is called before the first frame update
    void Start()
    {
        myCurrentStamina = myMaxStamina;
        myStaminaImage = GameObject.Find("Fill").GetComponent<Image>();
        //mySliderStamina.maxValue = myMaxStamina;
        //mySliderStamina.value = myMaxStamina;

        myPlayerController = GameObject.Find("PlayerPrefab").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!myPlayerController.GetIsRunning())
        {
            StaminaDepletion();
        }
    }

    public void StaminaDepletion()
    {
        if (myPlayerController.myIsAlive)
        {
            float isFlyingUpwards = Input.GetAxisRaw("Vertical");

            if (!myPlayerController.GetInverted())
            {
                if (isFlyingUpwards > 0)
                {
                    myCurrentStamina -= myStaminaDrain * Time.deltaTime;

                    //print(myCurrentStamina);
                }

                if (isFlyingUpwards == 0)
                {
                    myCurrentStamina -= myStaminaDrainAlways * Time.deltaTime;
                }

                if (isFlyingUpwards < 0)
                {
                    myCurrentStamina -= myStaminaDrainAlways * 0;
                }

            }
            else
            {
                if (isFlyingUpwards < 0)
                {
                    myCurrentStamina -= myStaminaDrain * Time.deltaTime;

                    //print(myCurrentStamina);
                }

                if (isFlyingUpwards == 0)
                {
                    myCurrentStamina -= myStaminaDrainAlways * Time.deltaTime;
                }

                if (isFlyingUpwards > 0)
                {
                    myCurrentStamina -= myStaminaDrainAlways * 0;
                }
            }
            if (myCurrentStamina <= 0)
            {
                myCurrentStamina = myMinStamina;
            }
        }

        myStaminaImage.fillAmount = myCurrentStamina / myMaxStamina;
    }

    public void RefillStamina()
    {
        myCurrentStamina = myMaxStamina;
        myStaminaImage.fillAmount = myCurrentStamina / myMaxStamina;
    }

    public float GetStaminaValue()
    {
        return myCurrentStamina;
    }
}