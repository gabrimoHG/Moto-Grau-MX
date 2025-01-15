using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableGate : MonoBehaviour
{
    Transform fenceA, fenceB;
    float reactionRot;
    [SerializeField]float gateAOpeningRot, gateBOpeningRot;
    [SerializeField]float gateAFrontAdditional, gateBFrontAdditional;
    [SerializeField]float gateABackAdditional, gateBBackAdditional;
    private void Awake()
    {
        fenceA = transform.GetChild(0);
        fenceB = transform.GetChild(1);
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
            //Debug.Break();
            Vector3 impactForce = collision.impulse / Time.fixedDeltaTime;
            impactForce /= 1000;

            // Get the first contact point of the collision
            Vector3 collisionPoint = collision.contacts[0].point;
            
            // Convert collision point to the fence's local space
            Vector3 localCollisionPoint = transform.InverseTransformPoint(collisionPoint);

            //Debug.Log("Local collision in X: " + localCollisionPoint.x);

            if (localCollisionPoint.x > 0)
            {
                //Debug.Log("Impact from the front");
                if (impactForce.magnitude <= 35)
                {
                    //GetComponent<BoxCollider>().enabled = false;
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
                        fenceA, -gateAOpeningRot-gateAFrontAdditional, .5f, null, true));
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceB, gateBOpeningRot+gateBFrontAdditional, .5f, EnableOpenedGateColliders, true));
                }
            }
            else
            {
                //Debug.Log("Impact from the back");
                if (impactForce.magnitude <= 35)
                {
                    //GetComponent<BoxCollider>().enabled = false;
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
                        fenceA, gateAOpeningRot+gateABackAdditional, .5f, null, true));
                    StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceB, -gateBOpeningRot-gateBBackAdditional, .5f, EnableOpenedGateColliders, true));
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

    void EnableOpenedGateColliders()
    {
        fenceA.GetChild(0).GetComponent<Collider>().enabled = true;
        fenceB.GetChild(0).GetComponent<Collider>().enabled = true;
    }
    void ReboundRotationFrontal()
    {
        var reboundRot = Random.Range(gateAOpeningRot / 20, gateAOpeningRot / 10);
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceA, -gateAOpeningRot + reboundRot, .1f, null, true));
        Debug.Log("A rebound " + (-gateAOpeningRot - reboundRot));

        reboundRot = Random.Range(gateBOpeningRot / 20, gateBOpeningRot / 10);
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
            fenceB, gateBOpeningRot - reboundRot, .1f, null, true));
        Debug.Log("B rebound " + (gateBOpeningRot - reboundRot));
    }
    void ReboundRotationBack()
    {
        var reboundRot = Random.Range(gateAOpeningRot / 20, gateAOpeningRot / 10);
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
                        fenceA, -gateAOpeningRot - reboundRot, .1f, null, true));
        
        reboundRot = Random.Range(gateBOpeningRot / 20, gateBOpeningRot / 10);
        StartCoroutine(InterpolationUtilities.RotateInYLinearly(
            fenceB, gateBOpeningRot + reboundRot, .1f, null, true));
    }
    //void EnableCentralCollider()=> GetComponent<BoxCollider>().enabled = true;
}
