using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeBP_Pro;
using HistoricGamesUtils;
public class WheelieRating : MonoBehaviour
{
    ArcadeBikeControllerPro bikeController;
    public float executionTime {  get; private set; }
    public float averageAngle {  get; private set; }
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
        float startTime = Time.fixedTime;
        float speedsRecorded = 0;
        float wheelieAngles = 0;
        int wheelieCycles = 0;
        //Debug.Log("Is doing wheelie? "+ bikeController.isDoingWheelie);
        while (bikeController.isDoingWheelie)
        {
            var currentAngle = Mathf.Abs(bikeController.bikeReferences.
                wheelieTransform.localRotation.eulerAngles.x);
            var currentSpeed = bikeController.speedKmph;

            speedsRecorded += currentSpeed;
            wheelieAngles += currentAngle;
            wheelieCycles++;
            
            yield return null;
        }

        float endTime = Time.fixedTime;
        executionTime = endTime - startTime;
        averageAngle = wheelieAngles / wheelieCycles;
        averageAngle = GeneralUtilities.
            ConvertAngleToFirstHalfStandard(averageAngle);
        averageSpeed = speedsRecorded / wheelieCycles;
        hasPerformed = true;
    }

    void ShowStatistics(bool hasSuceeded)
    {
        if (hasSuceeded && hasPerformed)
        {
            Debug.Log("Wheelie time: " + executionTime + " seconds.");
            Debug.Log("Wheelie average angle: " + averageAngle);
            Debug.Log("Wheelie average speed: " + averageSpeed);
            hasPerformed = false;
            bikeController.bikeEvents.onWheelieFinish -= ShowStatistics;
            bikeController.bikeEvents.onWheelieStart += TriggerWheelieStatistics;
            StopCoroutine(RecordWheelieStatistics());
        }
    }
}
