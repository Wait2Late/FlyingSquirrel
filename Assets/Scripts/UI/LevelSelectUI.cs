using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    bool myOnMouseHover = false;

    //public Button button;
    public void ExpandImage(Image aButtonImage)
    {
        aButtonImage.rectTransform.sizeDelta = new Vector2(aButtonImage.rectTransform.sizeDelta.x + 35, aButtonImage.rectTransform.sizeDelta.y + 35);
    }
    public void ShrinkImage(Image aButtonImage)
    {
        aButtonImage.rectTransform.sizeDelta = new Vector2(aButtonImage.rectTransform.sizeDelta.x - 35, aButtonImage.rectTransform.sizeDelta.y - 35);
    }

    float maxSize = 1.5f;
    float growFactor = 50f;

    public void Appear(GameObject anInfoObject)
    {
        anInfoObject.SetActive(true);
        myOnMouseHover = true;
        StartCoroutine(Enlarge(anInfoObject));
    }

    public void Disappear(GameObject anInfoObject)
    {
        myOnMouseHover = false;
        StartCoroutine(Minimize(anInfoObject));
    }

    IEnumerator Enlarge(GameObject anInfoObject)
    {
        while (maxSize > anInfoObject.transform.localScale.x)
        {
            if (!myOnMouseHover)
            {
                break;
            }
            anInfoObject.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;

            if (anInfoObject.transform.localScale.x > maxSize)
            {
                anInfoObject.transform.localScale = new Vector3(maxSize, maxSize, maxSize);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator Minimize(GameObject anInfoObject)
    {
        while (0.1f < anInfoObject.transform.localScale.x)
        {
            if (myOnMouseHover)
            {
                break;
            }
            anInfoObject.transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime *20;
            yield return new WaitForSeconds(0.001f);
        }

        anInfoObject.SetActive(false);
    }
}