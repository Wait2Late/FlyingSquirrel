using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void GoBackToMainMenuFromSettings(GameObject anImage)
    {
        anImage.SetActive(false);
        
    }

    public void DisableHighlightOnClick(GameObject anImage)
    {
        anImage.SetActive(false);

    }

    public void OnExit(GameObject anImage)
    {
        //anImage.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
