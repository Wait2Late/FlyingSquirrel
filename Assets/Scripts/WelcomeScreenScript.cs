using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WelcomeScreenScript : MonoBehaviour
{
    [SerializeField] private GameObject myText;
    [SerializeField] private float myTextScaleFactor = 20f;
    [SerializeField] private float myTextScaleBase = .6f;
    private float myTargetAlpha = 0f;
    private CanvasGroup myCanvasGroup;
    private string myNextScene = "Main Menu";

    private void Start()
    {
        myCanvasGroup = myText.GetComponent<CanvasGroup>();
        myCanvasGroup.alpha = 0f;
        myText.GetComponent<Text>().fontSize = 100;

        UnityEngine.Video.VideoPlayer videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();

        videoPlayer.loopPointReached += EndReached;

        SceneManager.LoadSceneAsync(myNextScene).allowSceneActivation = false;
    }
    private void Update()
    {
        if (myCanvasGroup.alpha != myTargetAlpha)
        {
            myCanvasGroup.alpha = Mathf.Lerp(myCanvasGroup.alpha, myTargetAlpha, .01f);
        }

        myText.transform.localScale = new Vector3((Mathf.Abs(Mathf.Sin(Time.time)) / myTextScaleFactor) + myTextScaleBase, (Mathf.Abs(Mathf.Sin(Time.time)) / myTextScaleFactor) + myTextScaleBase, 0);

        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(myNextScene);
        }
    }
    private void EndReached(UnityEngine.Video.VideoPlayer aVideoPlayer)
    {
        myTargetAlpha = 1f;
    }
}
