using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArcadeBP_Pro.ArcadeBikeControllerPro;

public class DummyBike : MonoBehaviour
{
    AudioSource engineSound;
    AudioSource skidSound;
    // Start is called before the first frame update
    void Awake()
    {
        engineSound = transform.GetChild(4).GetChild(0).GetComponent<AudioSource>();
        skidSound = transform.GetChild(4).GetChild(1).GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine(OverRev(.5f));
    }

    private IEnumerator OverRev(float duration)
    {
        float elapsedTime = 0f;
        var currentBike = GameManager.instance.GetCurrentBike();
        var currentPitch = currentBike.bikeAudio.engineSound.pitch;
        var currentVolume = currentBike.bikeAudio.engineSound.volume;
        engineSound.time = currentBike.bikeAudio.engineSound.time;
        engineSound.Play();

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            //Debug.Log("Entrou!");
            // Lerp the pitch and volume towards the over-rev max values
            engineSound.pitch = Mathf.Lerp(currentPitch, (currentPitch + 1), t);
            if (currentVolume != 1)
            {
                var differenceToOne = 1 - currentVolume;
                engineSound.volume = Mathf.Lerp(currentVolume, differenceToOne, t);
            }
            else
                engineSound.volume = currentVolume;

            yield return null;
        }

        StartCoroutine(SkidAndEngineIdle(duration));
    }

    IEnumerator SkidAndEngineIdle(float duration)
    {
        float elapsedTime = 0f;
        var currentPitch = engineSound.pitch;
        var currentVolume = engineSound.volume;
        skidSound.Play();
        //Debug.Log("Skid sound volume " + skidSound.volume);
        //Debug.Log("Skid sound pitch " + skidSound.pitch);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            engineSound.pitch = Mathf.Lerp(currentPitch, .2f, t);
            engineSound.volume = Mathf.Lerp(currentVolume, .5f, t);
            yield return null;
        }
        skidSound.Stop();
    }

}
