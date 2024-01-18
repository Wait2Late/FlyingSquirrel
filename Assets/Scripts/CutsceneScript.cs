using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneScript : MonoBehaviour
{
    [SerializeField] private string mySceneToLoad = "WelcomeScreenScene";
    private void Start()
    {
        UnityEngine.Video.VideoPlayer videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer.loopPointReached += EndReached;

        SceneManager.LoadSceneAsync(mySceneToLoad).allowSceneActivation = false;
    }
    private void EndReached(UnityEngine.Video.VideoPlayer aVideoPlayer)
    {
        SceneManager.LoadScene(mySceneToLoad);
    }
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(mySceneToLoad);
        }
    }
}
