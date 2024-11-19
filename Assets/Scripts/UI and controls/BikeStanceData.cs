using ArcadeBP_Pro;
using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BikeStanceData : MonoBehaviour
{
    Image silhouette;
    TextMeshProUGUI executionTime, averageAngle;
    WheelieRating wheelieRating;
    ArcadeBikeControllerPro bikeController;
    [SerializeField] float silhouetteFeedbackSpeed;
    Color minColor, maxColor;

    private void Awake()
    {
        silhouette = transform.GetChild(0).GetComponent<Image>();
        executionTime = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        averageAngle = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        minColor = Color.green;
        maxColor = Color.red;
    }
    void Start() 
    {
        bikeController = FindObjectOfType<ArcadeBikeControllerPro>();
        bikeController.bikeEvents.onWheelieStart += TriggerStanceUIUpdate;
        bikeController.bikeEvents.onWheelieFinish += ResetWheelieData;
        GameManager.instance.onPlayerBikeChange += UpdateBikeReferences;
        silhouette.sprite = bikeController.bikeReferences.dataContainer.silhouette;
    }

    public void UpdateBikeReferences(ArcadeBikeControllerPro bikeController)
    {
        StopAllCoroutines();
        this.bikeController = bikeController;
        silhouette.sprite = bikeController.bikeReferences.dataContainer.silhouette;
        bikeController.bikeEvents.onWheelieStart += TriggerStanceUIUpdate;
        bikeController.bikeEvents.onWheelieFinish += ResetWheelieData;
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
        
        var wheelieLimit = bikeController.bikeSettings.wheelieAngleLimit;
        wheelieRating = GameManager.instance.wheelieRating;
        if(!bikeController.isDoingWheelie)
            yield return null;
        while (bikeController.isDoingWheelie)
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
