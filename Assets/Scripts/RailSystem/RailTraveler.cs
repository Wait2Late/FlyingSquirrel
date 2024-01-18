using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RailTraveler : MonoBehaviour
{
    [SerializeField] string myLevelOneScene;
    [SerializeField] string myLevelTwoScene;
    [SerializeField] string myLevelThreeScene;

    string mySelectedLevel;

    [SerializeField] BezierCurveGenerator myCurrentCurveGenerator;
  
    [SerializeField] float mySpeed = 5;

    [SerializeField] bool myRotateAlongCurve;

    [SerializeField] bool myStartMoving;

    float myDistanceTraveled = 0;
    float myTotalDistanceOfCurve;

    bool myIsTraveling;

    [SerializeField] LoadSceneOnClick mySceneLoader;

    [SerializeField] LevelSelectController mylevelSelectController;

    private void Start()
    {
        myIsTraveling = myStartMoving;

        mylevelSelectController = GameObject.Find("LevelSelectCanvas").GetComponent<LevelSelectController>();
    }

    private void Update()
    {
        if (myIsTraveling)
        {
            if (myDistanceTraveled>=myTotalDistanceOfCurve)
            {
                mySceneLoader.LoadOnClick(mySelectedLevel);
            }
            else
            {
                myDistanceTraveled += Time.deltaTime * mySpeed;
                Vector3[] newPosAndRot = myCurrentCurveGenerator.TravelOnPath(myDistanceTraveled);
                transform.position = newPosAndRot[0];

                if (myRotateAlongCurve)
                {
                    transform.rotation = Quaternion.LookRotation(newPosAndRot[1]);
                }
            }           
        }      
    }

    public void SetCurrentCurve(BezierCurveGenerator aCurve)
    {
        myCurrentCurveGenerator = aCurve;
        myIsTraveling = true;
        myTotalDistanceOfCurve = myCurrentCurveGenerator.GetLengthOfCurve();
        mySpeed = myCurrentCurveGenerator.GetLengthOfCurve() / mylevelSelectController.GetTransitionTime();
    }

    public void SetSelectedScene(string aScene)
    {
        GameObject.Find("GameManager").GetComponent<PreloadScript>().Load(aScene);
        mySelectedLevel = aScene;
    }
}
