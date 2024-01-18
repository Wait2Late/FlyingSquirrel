using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreloadScript : MonoBehaviour
{
    public static PreloadScript ourInstance { get; private set; }
    private void Awake()
    {
        if (ourInstance == null)
        {
            ourInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool myIsCurrentlyLoading = false;

    public void Load(string aName)
    {
        if (myIsCurrentlyLoading) return;
        StartCoroutine(LoadAsyncScene(aName));
    }

    private IEnumerator LoadAsyncScene(string aName)
    {
        print("Preloading " + aName);
        AsyncOperation async = SceneManager.LoadSceneAsync(aName);
        async.allowSceneActivation = false;
        myIsCurrentlyLoading = true;
        while (!async.isDone)
        {
            yield return null;
        }
        myIsCurrentlyLoading = false;
    }
}
