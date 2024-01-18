using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsCounter;
    Queue<float> fpsQueue = new Queue<float>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float fps = (1 / Time.deltaTime);

        fpsQueue.Enqueue(fps);

        if (fpsQueue.Count > 5)
        {
            fpsQueue.Dequeue();
        }

        float average = fpsQueue.Sum() / fpsQueue.Count;

        int iAverage = (int)Math.Round(average, 0);

        fpsCounter.text = iAverage+"";
    }
}
