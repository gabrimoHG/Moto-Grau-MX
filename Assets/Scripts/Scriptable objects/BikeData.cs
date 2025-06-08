using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New bike", menuName = "ScriptableObjects/Bike data", order = 1)]
public class BikeData : GameData
{
    [Header("Base references")]
    public GameObject mainPrefab;
    public GameObject dummyPrefab;
    public Sprite silhouette;
    public Material bodyMaterial;
    public Material rimMaterial;
    
    [Serializable]
    public class Geometry
    {
        [Tooltip("Radius of the front wheel. Affects the ground contact area and suspension calculations.")]
        public float frontWheelRadius;

        [Tooltip("Width of the front wheel.")]
        public float frontWheelWidth;

        [Tooltip("Radius of the rear wheel.")]
        public float rearWheelRadius;

        [Tooltip("Width of the rear wheel.")]
        public float rearWheelWidth;

        [Tooltip("Angle of the front wheel relative to the bike’s body. Affects the bike’s suspension and wheel placement calculations.")]
        public float frontWheelAngle;

        [Tooltip("Angle of the rear wheel relative to the bike’s body. Similar to the front wheel angle but for the rear wheel.")]
        public float rearWheelAngle;
    }
    public Geometry geometry;
    [Serializable]
    public class Suspension
    {
        [Tooltip("The force exerted by the bike’s suspension springs. Higher values result in a stiffer suspension.")]
        public float springForce;

        [Tooltip("The force exerted by the dampers to absorb shocks. Higher values result in less oscillation.")]
        public float damperForce;

        [Tooltip("A factor to adjust how well the bike sticks to the ground.")]
        public float groundStickFactor;

        [Tooltip("Maximum allowable compression for the suspension. Prevents the bike from bottoming out.")]
        public float maxCompression;
    }
    public Suspension suspension;

    [Serializable]
    public class MainSettings
    {
        [Tooltip("LayerMask for drivable surfaces. Remove any layer that you dont want bike to drive on")]
        public LayerMask drivableLayerMask;

        [Tooltip("Maximum speed the bike can reach in the forward direction.")]
        public float maxSpeed;

        [Tooltip("Maximum speed the bike can reach in reverse.")]
        public float reverseMaxSpeed;

        [Tooltip("Acceleration rate of the bike.")]
        public float acceleration;

        [Tooltip("Acceleration rate when the bike is in reverse.")]
        public float reverseAcceleration;

        public int horsepower;

        [Tooltip("Rate at which the bike decelerates when opposite button(to the bike move direction) is pressed. Basically brake amount. How fast the bike will reach 0 speed.")]
        public float deceleration;

        [Tooltip("Deceleration rate when the hand brake is applied.")]
        public float handBrakeDeceleration;

        [Tooltip("Maximum angle the bike handle can turn.")]
        public float maxTurnAngle;

        [Tooltip("Whether to use smooth turning. Sometimes you don't want instant left to right turning. It delays how fast we can turn the bike by inputs.")]
        public bool useLerpTurning;

        [Tooltip("Speed of smooth turning if `useLerpTurning` is true.")]
        public float turnLerpSpeed;

        [Tooltip("Speed of the steering animation.")]
        public float steeringAnimationSpeed;

        [Tooltip("Whether the bike can steer while in the air (not grounded).")]
        public bool canSteerInAir;

        [Tooltip("Whether the bike can lean while in the air (not grounded).")]
        public bool canLeanInAir;

        [Tooltip("Speed of turning while in the air if `canSteerInAir` is true.")]
        public float turnSpeedInAir;

        [Tooltip("Maximum lean angle for the bike.")]
        public float maxLeanAngle;

        [Tooltip("Speed of the leaning animation.")]
        public float leaningAnimationSpeed;

        [Tooltip("Coefficient for calculating friction force (sideways force).")]
        public float frictionCoefficient;

        [Tooltip("Factor to adjust friction during drifts (when handbrake is applied).")]
        public float driftFrictionFactor;

        [Tooltip("Factor to adjust turning during drifts (when handbrake is applied).")]
        public float driftTurnFactor;

        [Tooltip("Resistance applied to the bike to simulate rolling friction. It determines how fast bike will come to rest (0 speed) if acceleration/deceleration input is not applied.")]
        public float rollingResistance;

        [Tooltip("Gravity force applied to the bike.")]
        public float gravity;

        [Tooltip("Speed of rotation during a burnout.")]
        public float burnoutRotationSpeed;

        [Tooltip("Smoothness of the burnout transition.")]
        public float burnoutSmoothness;

        [Tooltip("Speed of aligning the rotator when the bike is on the ground. How fast bike align with the ground surface.")]
        public float alignRotatorSpeed_Ground;

        [Tooltip("Speed of aligning the rotator when the bike is in the air. How fast bike align with the world up axis.")]
        public float alignRotatorSpeed_Air;
    }
    public MainSettings mainSettings;

    [Serializable]
    public class AdditionalSettings
    {
        [Tooltip("Maximum angle for performing a wheelie.")]
        public float wheelieAngle;

        public float wheelieAngleLimit;

        public float wheelieMinSpeed;
        public float wheelieMaxSpeed;
        public float minWheelieDropSpeed;
        public float rearWheelRayZOffset;
        //public float frontWheelRayReductionRange;
        public float brokenOrNoGasSpeedLimit;
        public float fuelConsumptionRate;
        public float extraConsumptionFloor;
        public float frontalWeight;
        public float scrapingDownForce;

        [Tooltip("Speed of the wheelie animation.")]
        public float wheelieAnimationSpeed;

        [Tooltip("Array of speeds for each gear. The bike will shift gears based on its current speed in kmph.")]
        public float[] gearSpeeds;

        [Tooltip("The current gear of the bike.")]
        public int mechanicalGear;

        public int logicalGear;
    }
    public AdditionalSettings additionalSettings;

    [Serializable]
    public class BehaviorCurves
    {
        [Tooltip("Adjusts the acceleration. X-axis represents speed, Y-axis represents acceleration multiplier. Example: Curve from (0, 1) to (1, 0.5) means high acceleration at low speed, decreasing to half acceleration at max speed.")]
        public AnimationCurve accelerationCurve;

        [Tooltip("Adjusts the reverse acceleration. X-axis represents reverse speed, Y-axis represents acceleration multiplier.")]
        public AnimationCurve reverseAccelerationCurve;

        [Tooltip("Adjusts the steering sensitivity. X-axis represents speed, Y-axis represents steering sensitivity. Example: Curve from (0, 1) to (1, 0.3) means sensitive steering at low speed, decreasing sensitivity at high speed. You will have to adjust it with multiple points to fine tune the turning.")]
        public AnimationCurve steeringCurve;

        [Tooltip("Adjusts friction based on speed. X-axis represents sideways speed, Y-axis represents friction multiplier. Example: Curve from (0, 1) to (1, 0.1) means high friction at low sideways speed (meaning when bike is not sliding sideways), decreasing to low friction at high sideways speed (meaning when bike is sliding sideways).")]
        public AnimationCurve frictionCurve;

        [Tooltip("Adjusts the lean angle based on speed. X-axis represents speed, Y-axis represents lean angle multiplier. Example: Curve from (0, 0.2) to (1, 1) means minimal lean at low speed, increasing to maximum lean at high speed.")]
        public AnimationCurve leanCurve;

        public AnimationCurve wheelieEasinessCurve;
    }
    public BehaviorCurves behaviorCurves;
}
