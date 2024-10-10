using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeBP_Pro;
using HistoricGamesUtils;
public class WheelieRating : MonoBehaviour
{
    ArcadeBikeControllerPro bikeController;
    float startTime;
    float speedsRecorded;
    float wheelieAngles;
    int wheelieCycles;
    public float executionTime {  get; private set; }
    public float averageAngle {  get; private set; }
    public float currentAngle { get; private set; }
    public float averageSpeed { get; private set; }
    
    bool hasPerformed;
    // Start is called before the first frame update
    void Start()
    {
        bikeController = FindObjectOfType<ArcadeBikeControllerPro>();
        bikeController.bikeEvents.onWheelieStart += TriggerWheelieStatistics;
        //bikeController.bikeEvents.onWheelieFinish += ShowStatistics;
    }

    void TriggerWheelieStatistics() => StartCoroutine(RecordWheelieStatistics());

    IEnumerator RecordWheelieStatistics()
    {
        bikeController.bikeEvents.onWheelieStart -= TriggerWheelieStatistics;
        bikeController.bikeEvents.onWheelieFinish += ShowStatistics;
        executionTime = 0f;
        startTime = Time.fixedTime;
        speedsRecorded = 0;
        wheelieAngles = 0;
        wheelieCycles = 0;
        //Debug.Log("Is doing wheelie? "+ bikeController.isDoingWheelie);
        while (bikeController.isDoingWheelie)
        {
            currentAngle = Mathf.Abs(bikeController.bikeReferences.
                wheelieTransform.localRotation.eulerAngles.x);
            var currentSpeed = bikeController.speedKmph;

            speedsRecorded += currentSpeed;
            wheelieAngles += currentAngle;
            wheelieCycles++;
            UpdateGlobalData();

            yield return null;
        }

        
        hasPerformed = true;
    }

    void UpdateGlobalData()
    {
        float endTime = Time.fixedTime;
        executionTime = endTime - startTime;
        averageAngle = 360-(wheelieAngles / wheelieCycles);
        averageSpeed = speedsRecorded / wheelieCycles;
    }

    void ShowStatistics(bool hasSuceeded)
    {
        if (hasSuceeded && hasPerformed)
        {
            //Debug.Log("Wheelie time: " + executionTime + " seconds.");
            //Debug.Log("Wheelie average angle: " + averageAngle);
            //Debug.Log("Wheelie average speed: " + averageSpeed);
            bikeController.bikeEvents.onWheelieFinish -= ShowStatistics;
            bikeController.bikeEvents.onWheelieStart += TriggerWheelieStatistics;
            StopCoroutine(RecordWheelieStatistics());
        }
        hasPerformed = false;
    }
}
