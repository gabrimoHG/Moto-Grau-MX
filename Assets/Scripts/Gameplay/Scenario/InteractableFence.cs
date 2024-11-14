using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableFence : MonoBehaviour
{
    Transform fenceA, fenceB;
    float reactionRot, openingRot;
    private void Awake()
    {
        fenceA = transform.GetChild(0);
        fenceB = transform.GetChild(1);
        openingRot = 100;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*private void(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            other
            Debug.Log("Player entrour no trigger!");
        }
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Debug.Break();
            Vector3 impactForce = collision.impulse / Time.fixedDeltaTime;
            impactForce /= 1000;

            // Get the first contact point of the collision
            Vector3 collisionPoint = collision.contacts[0].point;

            // Convert collision point to the fence's local space
            Vector3 localCollisionPoint = transform.InverseTransformPoint(collisionPoint);

            // Check if the collision happened in front or back (using the local Z-axis)
            if (localCollisionPoint.z > 0)
            {
                Debug.Log("Impact from the front");
                if (impactForce.magnitude <= 50)
                {
                    GetComponent<BoxCollider>().enabled = false;
                    reactionRot = (impactForce.magnitude / 50f) * 10f;
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceA, -reactionRot, .25f, null, true));
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceB, reactionRot, .25f, ResetRotation, true));
                }
                else
                {
                    GetComponent<BoxCollider>().enabled = false;
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceA, -openingRot, .5f, null, true));
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceB, openingRot, .5f, ReboundRotationFrontal, true));
                }
            }
            else
            {
                if (impactForce.magnitude <= 50)
                {
                    GetComponent<BoxCollider>().enabled = false;
                    reactionRot = (impactForce.magnitude / 50f) * 10f;
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceA, reactionRot, .25f, null, true));
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceB, -reactionRot, .25f, ResetRotation, true));
                }
                else
                {
                    GetComponent<BoxCollider>().enabled = false;
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceA, openingRot, .5f, null, true));
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceB, -openingRot, .5f, ReboundRotationBack, true));
                }
            }
        }
    }

    void ResetRotation()
    {
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
            fenceA, 0, .25f, null, true));
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
            fenceB, 0, .25f, null, true));
    }
    void ReboundRotationFrontal()
    {
        var reboundRot = Random.Range(openingRot / 20, openingRot / 10);
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceA, -openingRot+reboundRot, .1f, null, true));
        Debug.Log("A rebound " + (-openingRot - reboundRot));

        reboundRot = Random.Range(openingRot / 20, openingRot / 10);
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
            fenceB, openingRot-reboundRot, .1f, null, true));
        Debug.Log("B rebound " + (openingRot - reboundRot));
    }
    void ReboundRotationBack()
    {
        var reboundRot = Random.Range(openingRot / 20, openingRot / 10);
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceA, -openingRot - reboundRot, .1f, null, true));
        
        reboundRot = Random.Range(openingRot / 20, openingRot / 10);
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
            fenceB, openingRot+reboundRot, .1f, null, true));
    }
    //void EnableCentralCollider()=> GetComponent<BoxCollider>().enabled = true;
}
