using ArcadeBP_Pro;
using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CollectibleItem : MonoBehaviour
{
    public event Action<CollectibleItem> onCollected = delegate { };
    [SerializeField] protected int powerUpAmount;
    protected AudioSource audioSource;
    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    protected virtual void OnEnable()
    {
        GetComponent<MeshRenderer>().enabled = true;
        IconMovementUp();
        IconRotation();
    }
    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }
    
    protected virtual void IconMovementUp()
    {
        var currentPostion = transform.position;
        var targetPosition = new Vector3(currentPostion.x,
            currentPostion.y + .5f, currentPostion.z);
        StartCoroutine(InterpolationUtilities.MoveToTargetLinearly(transform,
            currentPostion, targetPosition, .5f, IconMovementDown));
    }

    protected virtual void IconMovementDown()
    {
        var currentPostion = transform.position;
        var targetPosition = new Vector3(currentPostion.x,
            currentPostion.y - .5f, currentPostion.z);
        StartCoroutine(InterpolationUtilities.MoveToTargetLinearly(transform,
            currentPostion, targetPosition, .5f, IconMovementUp));
    }
    protected virtual void IconRotation()
    {
        var startRotation = transform.rotation;
        var endRotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + 20, transform.rotation.eulerAngles.z);
        StartCoroutine(InterpolationUtilities.RotateToTargetLinearly(
            transform, startRotation, endRotation, .5f, IconRotation));
    }

    protected IEnumerator AudioFeedbackAndEventTrigger()
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        onCollected(this);
        gameObject.SetActive(false);
    }
}
