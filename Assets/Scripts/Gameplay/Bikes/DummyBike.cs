using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DummyBike : MonoBehaviour
{
    [SerializeField]BikeData bikeData;
    
    public BikeData BikeData { get { return bikeData; } }
    public int condition, fuelAmount;
    AudioSource engineSound;
    AudioSource skidSound;
    AudioSource crashSound;
    public bool isShowcase;
    public bool isNew;

    // Start is called before the first frame update
    void Awake()
    {
        if (transform.childCount > 4)
        {
            engineSound = transform.GetChild(4).GetChild(0).GetComponent<AudioSource>();
            skidSound = transform.GetChild(4).GetChild(1).GetComponent<AudioSource>();
            crashSound = transform.GetChild(4).GetChild(2).GetComponent<AudioSource>();
        }
        else
            engineSound = transform.GetChild(1).GetChild(0).GetComponent<AudioSource>();
    }
    private void Start()
    {
        engineSound.Stop();
    }

    private void OnEnable()
    {
        if (!isShowcase)
            StartCoroutine(OverRev(.25f));
    }

    private IEnumerator OverRev(float duration)
    {
        float elapsedTime = 0f;
        var bikePhysicsController = FindObjectOfType<GameManager>().GetCurrentBikePhysicsController();//Revisado!
        var currentPitch = bikePhysicsController.bikeAudio.engineSound.pitch;
        var currentVolume = bikePhysicsController.bikeAudio.engineSound.volume;
        engineSound.time = bikePhysicsController.bikeAudio.engineSound.time;
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

        StartCoroutine(Skid(duration));
        StartCoroutine(CrashAndEngineIdle());
    }

    IEnumerator Skid(float duration)
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

    IEnumerator CrashAndEngineIdle()
    {
        var currentPitch = engineSound.pitch;
        var currentVolume = engineSound.volume;
        crashSound.Play();
        while (crashSound.isPlaying)
            yield return null;
    }

    public void SetRevEngine() => StartCoroutine(RevEngine());
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
