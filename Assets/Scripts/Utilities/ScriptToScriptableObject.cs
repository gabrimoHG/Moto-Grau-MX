using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScriptToScriptableObject : MonoBehaviour
{
    public BikeData bikeData; // Reference to your ScriptableObject
    public bool setData;
    private void Update()
    {
        // Ensure this only runs in edit mode
        if (!Application.isPlaying && setData)
            CopyDataToScriptableObject();
    }

    private void CopyDataToScriptableObject()
    {
        // Reference to the component in the scene
        var bikeController = GetComponent<ArcadeBikeControllerPro>();

        //Copy data from the regular script to the ScriptableObject
        bikeData.rimMaterial = bikeController.bikeMaterials.rimMaterial;
        bikeData.bodyMaterial = bikeController.bikeMaterials.bodyMaterial;

        bikeData.geometry.rearWheelRadius = 
            bikeController.bikeGeometry.rearWheelRadius;
        bikeData.geometry.rearWheelWidth = 
            bikeController.bikeGeometry.rearWheelWidth;
        bikeData.geometry.rearWheelAngle = 
            bikeController.bikeGeometry.rearWheelAngle;
        bikeData.geometry.frontWheelRadius = 
            bikeController.bikeGeometry.frontWheelRadius;
        bikeData.geometry.frontWheelWidth = 
            bikeController.bikeGeometry.frontWheelWidth;
        bikeData.geometry.frontWheelAngle = 
            bikeController.bikeGeometry.frontWheelAngle;

        bikeData.suspension.springForce = 
            bikeController.bikeSuspension.springForce;
        bikeData.suspension.damperForce = 
            bikeController.bikeSuspension.damperForce;
        bikeData.suspension.groundStickFactor =
            bikeController.bikeSuspension.groundStickFactor;
        bikeData.suspension.maxCompression = 
            bikeController.bikeSuspension.maxCompression;

        bikeData.mainSettings.drivableLayerMask = 
            bikeController.bikeSettings.drivableLayerMask;
        bikeData.mainSettings.maxSpeed = 
            bikeController.bikeSettings.maxSpeed;
        bikeData.mainSettings.reverseMaxSpeed = 
            bikeController.bikeSettings.reverseMaxSpeed;
        bikeData.mainSettings.acceleration = 
            bikeController.bikeSettings.acceleration;
        bikeData.mainSettings.reverseAcceleration = 
            bikeController.bikeSettings.reverseAcceleration;
        bikeData.mainSettings.horsepower = 
            bikeController.bikeSettings.horsepower;
        bikeData.mainSettings.deceleration = 
            bikeController.bikeSettings.deceleration;
        bikeData.mainSettings.handBrakeDeceleration = 
            bikeController.bikeSettings.handBrakeDeceleration;
        bikeData.mainSettings.maxTurnAngle = 
            bikeController.bikeSettings.maxTurnAngle;
        bikeData.mainSettings.useLerpTurning = 
            bikeController.bikeSettings.useLerpTurning;
        bikeData.mainSettings.turnLerpSpeed = 
            bikeController.bikeSettings.turnLerpSpeed;
        bikeData.mainSettings.steeringAnimationSpeed = 
            bikeController.bikeSettings.steeringAnimationSpeed;
        bikeData.mainSettings.canSteerInAir = 
            bikeController.bikeSettings.canSteerInAir;
        bikeData.mainSettings.canLeanInAir = 
            bikeController.bikeSettings.canLeanInAir;
        bikeData.mainSettings.turnSpeedInAir = 
            bikeController.bikeSettings.turnSpeedInAir;
        bikeData.mainSettings.maxLeanAngle = 
            bikeController.bikeSettings.maxLeanAngle;
        bikeData.mainSettings.leaningAnimationSpeed = 
            bikeController.bikeSettings.leaningAnimationSpeed;
        bikeData.mainSettings.frictionCoefficient = 
            bikeController.bikeSettings.frictionCoefficient;
        bikeData.mainSettings.driftFrictionFactor = 
            bikeController.bikeSettings.driftFrictionFactor;
        bikeData.mainSettings.driftTurnFactor = 
            bikeController.bikeSettings.driftTurnFactor;
        bikeData.mainSettings.rollingResistance = 
            bikeController.bikeSettings.rollingResistance;
        bikeData.mainSettings.gravity = 
            bikeController.bikeSettings.gravity;
        bikeData.mainSettings.burnoutRotationSpeed = 
            bikeController.bikeSettings.burnoutRotationSpeed;
        bikeData.mainSettings.burnoutSmoothness = 
            bikeController.bikeSettings.burnoutSmoothness;
        bikeData.mainSettings.alignRotatorSpeed_Ground = 
            bikeController.bikeSettings.alignRotatorSpeed_Ground;
        bikeData.mainSettings.alignRotatorSpeed_Air = 
            bikeController.bikeSettings.alignRotatorSpeed_Air;

        bikeData.additionalSettings.wheelieAngle =
            bikeController.bikeSettings.wheelieAngle;
        bikeData.additionalSettings.wheelieAngleLimit =
            bikeController.bikeSettings.wheelieAngleLimit;
        bikeData.additionalSettings.wheelieMinSpeed =
            bikeController.bikeSettings.wheelieMinSpeed;
        bikeData.additionalSettings.wheelieMaxSpeed =
            bikeController.bikeSettings.wheelieMaxSpeed;
        bikeData.additionalSettings.minWheelieDropSpeed =
            bikeController.bikeSettings.minWheelieDropSpeed;
        bikeData.additionalSettings.rearWheelRayZOffset =
            bikeController.bikeSettings.rearWheelRayZOffset;
        bikeData.additionalSettings.brokenOrNoGasSpeedLimit =
            bikeController.bikeSettings.brokenOrNoGasSpeedLimit;
        bikeData.additionalSettings.fuelConsumptionRate =
            bikeController.bikeSettings.fuelConsumptionRate;
        bikeData.additionalSettings.extraConsumptionFloor =
            bikeController.bikeSettings.extraConsumptionFloor;
        bikeData.additionalSettings.frontalWeight =
            bikeController.bikeSettings.frontalWeight;
        bikeData.additionalSettings.scrapingDownForce =
            bikeController.bikeSettings.scrapingDownForce;
        bikeData.additionalSettings.wheelieAnimationSpeed =
            bikeController.bikeSettings.wheelieAnimationSpeed;
        bikeData.additionalSettings.gearSpeeds =
           bikeController.gearSpeeds;
        bikeData.additionalSettings.mechanicalGear =
            bikeController.mechanicalGear;
        bikeData.additionalSettings.logicalGear =
            bikeController.logicalGear;

        bikeData.behaviorCurves.accelerationCurve = 
            bikeController.bikeCurves.accelerationCurve;
        bikeData.behaviorCurves.reverseAccelerationCurve = 
            bikeController.bikeCurves.reverseAccelerationCurve;
        bikeData.behaviorCurves.steeringCurve = 
            bikeController.bikeCurves.steeringCurve;
        bikeData.behaviorCurves.frictionCurve = 
            bikeController.bikeCurves.frictionCurve;
        bikeData.behaviorCurves.leanCurve = 
            bikeController.bikeCurves.leanCurve;
        bikeData.behaviorCurves.wheelieEasinessCurve = 
            bikeController.bikeCurves.wheelieEasinessCurve;

        Debug.Log("ScriptableObject updated!");
        setData = false;
    }
}
