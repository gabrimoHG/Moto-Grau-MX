using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeKey : CollectibleItem
{
    AudioSource engineSound;

    protected override void OnEnable()
    {
        base.OnEnable();
        engineSound = GetComponent<AudioSource>();
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();//Revisado!
        if (other.transform.root.gameObject == bikePhysicsController.gameObject)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(AudioFeedbackAndEventTrigger());
            PlayerPrefs.SetInt("IsNextBikeKeyCollected",1);
            StartCoroutine(RevEngine());
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            //UIManager.instance.debugConsole.
            //    AddMessage("Refuel triggered!");
#endif
        }
    }

    private IEnumerator RevEngine()
    {
        float minPitch = 0.8f;
        float maxPitch = 1.5f;
        float minVolume = 0;
        float maxVolume = 1f;
        engineSound.volume = minVolume;
        engineSound.Play();

        float halfTime = 1.5f / 2f;
        float elapsedTime = 0f;

        // Revving up
        while (elapsedTime < halfTime)
        {
            engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, elapsedTime / halfTime);
            engineSound.volume = Mathf.Lerp(minVolume, maxVolume, elapsedTime / halfTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        // Revving down
        while (elapsedTime < halfTime)
        {
            engineSound.pitch = Mathf.Lerp(maxPitch, minPitch, elapsedTime / halfTime);
            engineSound.volume = Mathf.Lerp(maxVolume, minVolume, elapsedTime / halfTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        engineSound.pitch = minPitch;
        engineSound.volume = .5f;
        //yield return new WaitForSeconds(.5f);
        engineSound.Stop();
    }
}
