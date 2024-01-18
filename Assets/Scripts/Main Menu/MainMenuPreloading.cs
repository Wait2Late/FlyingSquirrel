using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPreloading : MonoBehaviour
{
    void Start()
    {
        GetComponent<PreloadScript>().Load("LevelSelect");
    }
}
