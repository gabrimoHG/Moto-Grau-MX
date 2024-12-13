using ArcadeBP_Pro;
using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BikeStanceData : MonoBehaviour
{
    GameManager gameManager;
    ArcadeBikeControllerPro bikePhysicsController;

    Image silhouette;
    TextMeshProUGUI executionTime, averageAngle;
    WheelieRating wheelieRating;
    [SerializeField] float silhouetteFeedbackSpeed;
    Color minColor, maxColor;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        silhouette = transform.GetChild(0).GetComponent<Image>();
        executionTime = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        averageAngle = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        minColor = Color.green;
        maxColor = Color.red;
    }
    void Start() 
    {
        bikePhysicsController = gameManager.GetCurrentBikePhysicsController();
        silhouette.sprite = bikePhysicsController.bikeReferences.dataContainer.silhouette;
        bikePhysicsController.events.onWheelieStart += TriggerStanceUIUpdate;
        bikePhysicsController.events.onWheelieFinish += ResetWheelieData;
        gameManager.onPlayerBikeChange += UpdateBikeReferences;//Revisado!
    }

    public void UpdateBikeReferences(ArcadeBikeControllerPro bikePhysicsController)
    {
        StopAllCoroutines();
        this.bikePhysicsController = bikePhysicsController;
        silhouette.sprite = bikePhysicsController.bikeReferences.dataContainer.silhouette;
        bikePhysicsController.events.onWheelieStart += TriggerStanceUIUpdate;
        bikePhysicsController.events.onWheelieFinish += ResetWheelieData;
        ResetWheelieData(false);
    }


    void TriggerStanceUIUpdate()
        =>StartCoroutine(SetSilhouetteRotationOnWheelie());

    void ResetWheelieData(bool hasSucceeded)
    {
        StopCoroutine(SetSilhouetteRotationOnWheelie());
        executionTime.text = "0:00";
        averageAngle.text = "0º";
        silhouette.color = Color.white;
        silhouette.rectTransform.rotation = Quaternion.identity;
    }

    IEnumerator SetSilhouetteRotationOnWheelie()
    {

        var wheelieLimit = bikePhysicsController.bikeSettings.wheelieAngleLimit;

        
        wheelieRating = gameManager.wheelieRating;
        if(!bikePhysicsController.isDoingWheelie)
            yield return null;
        while (bikePhysicsController.isDoingWheelie)
        {
            executionTime.text = GeneralUtilities.ConvertToTimeFormat(
                wheelieRating.executionTime).ToString();
            averageAngle.text = wheelieRating.averageAngle.ToString("F2") + "º";

            var targetRot = 360- wheelieRating.currentAngle;
            float currentRotationZ = silhouette.rectTransform.rotation.eulerAngles.z;
            float newRotationZ = Mathf.LerpAngle(currentRotationZ, targetRot,
                Time.deltaTime * silhouetteFeedbackSpeed);
            silhouette.rectTransform.rotation = Quaternion.Euler(0, 0, newRotationZ);

            float normalizedValue = Mathf.InverseLerp(0, wheelieLimit, targetRot);
            silhouette.color = Color.Lerp(minColor, maxColor, normalizedValue);

            yield return null;
        }
    }
}
