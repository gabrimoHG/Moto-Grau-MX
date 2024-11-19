using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;
using static UnityEngine.UI.Image;

public class TailSection : MonoBehaviour
{
    public bool isScraping {  get; private set; }
    public bool isBroken { get; private set; }
    public List<Transform> additionalPieces { get; private set; }
    public GameObject baseModel { get; private set; }
    [SerializeField] float resistanceDuration;
    [SerializeField] float tailResistance;
    [SerializeField] float scrapeTickInterval;
    BoxCollider boxCollider;
    AudioSource scrapingSound;
    Coroutine stopScrapingAudio, tailStatus;
    [SerializeField]ParticleSystem collisionParticle;
    Coroutine disableParticleCoroutine;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        additionalPieces = new List<Transform>();
        if (transform.parent != null)
        {
            baseModel = transform.parent.gameObject;
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (i == 0)
                    continue;
                additionalPieces.Add(transform.parent.GetChild(i));
            }
        }
    }

    void Start()
    {
        var bike = GameManager.instance.GetCurrentBikeController();
        scrapingSound = bike.bikeAudio.scrapingSound;
    }
    IEnumerator CheckTailStatus()
    {
        while(!isBroken)
        {
            if (tailResistance == 0)
            {
                boxCollider.isTrigger = false;
                boxCollider.includeLayers = (1 << 5) | (1 << 8) | (1 << 9);
                GameObject instance = null;
                var randomForce = 0;
                var force = Vector3.zero;

                for (int i = additionalPieces.Count - 1; i >= 0; i--)
                {
                    var pieceGameobject = additionalPieces[i].gameObject;
                    instance = Instantiate(pieceGameobject, pieceGameobject.transform.position,
                        pieceGameobject.transform.rotation);
                    instance.name = pieceGameobject.name;
                    instance.AddComponent<Rigidbody>();
                    randomForce = Random.Range(12, 15);
                    force = Vector3.back * randomForce;
                    instance.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                    instance.transform.parent = null;
                    GameManager.instance.SetDestruction(instance);
                    pieceGameobject.SetActive(false);
                }

                instance = Instantiate(baseModel.gameObject, baseModel.transform.position,
                    baseModel.transform.rotation);
                instance.name = baseModel.name;
                var instanceBoxCollider = instance.transform.GetChild(0).GetComponent<BoxCollider>();
                instanceBoxCollider.includeLayers = (1 << 3) | (1 << 6) | (1 << 7);
                instanceBoxCollider.gameObject.AddComponent<Rigidbody>();
                Destroy(instanceBoxCollider.GetComponent<TailSection>());
                instanceBoxCollider.transform.parent = null;
                instance.transform.parent = instanceBoxCollider.transform;
                randomForce = Random.Range(8, 10);
                force = Vector3.back * randomForce;
                instanceBoxCollider.gameObject.GetComponent<Rigidbody>().
                    AddForce(force, ForceMode.Impulse);
                GameManager.instance.SetDestruction(instance);
                baseModel.SetActive(false);

                isBroken = true;
                tailStatus = null;
                isScraping = false;
                scrapingSound.Stop();

            }
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isBroken)
            return;
        if (!scrapingSound.isPlaying)
            scrapingSound.Play();

        if (stopScrapingAudio != null)
        {
            StopCoroutine(stopScrapingAudio);
            stopScrapingAudio = null;
        }
        if(!collisionParticle.isPlaying)
            collisionParticle.Play();
        if (disableParticleCoroutine != null) 
        {
            StopCoroutine(disableParticleCoroutine);
            disableParticleCoroutine = null;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (isBroken)
            return;
        isScraping = true;
        tailResistance -= (100f / resistanceDuration) * Time.deltaTime;
        if(!scrapingSound.isPlaying)
            scrapingSound.Play();
        tailResistance = Mathf.Max(tailResistance, 0f);
        if (stopScrapingAudio != null)
            StopCoroutine(StopScrapingAudio());
        if (tailStatus == null)
            tailStatus = StartCoroutine(CheckTailStatus());
        stopScrapingAudio = StartCoroutine(StopScrapingAudio());
    }
    private void OnTriggerExit(Collider other)
    {
        if (isBroken)
            return;
        if (stopScrapingAudio == null)
            stopScrapingAudio = StartCoroutine(StopScrapingAudio());
        isScraping = false;
        if (disableParticleCoroutine != null)
        {
            StopCoroutine(disableParticleCoroutine);
            disableParticleCoroutine = null;
        }
        disableParticleCoroutine = StartCoroutine(DisableParticles());
        //Invoke(nameof(DisableParticles), 1);
    }

    private IEnumerator StopScrapingAudio()
    {
        yield return new WaitForSeconds(scrapeTickInterval);

        if (scrapingSound.isPlaying)
            scrapingSound.Stop();

        stopScrapingAudio = null;
    }

    IEnumerator DisableParticles()
    {
        yield return new WaitForSeconds(scrapeTickInterval/3);

        collisionParticle.Stop();
        disableParticleCoroutine = null;
    }
}
