using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;

public class TailSection : MonoBehaviour
{
    GameManager gameManager;
    public bool isBroken { get; private set; }
    public bool isScraping {  get; private set; }
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
        gameManager = FindObjectOfType<GameManager>();
        boxCollider = GetComponent<BoxCollider>();
        additionalPieces = new List<Transform>();
        if (transform.parent != null)
        {
            baseModel = transform.parent.gameObject;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i == 0)
                    continue;
                additionalPieces.Add(transform.GetChild(i));
            }
        }
    }

    void Start()
    {

        var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();//Revisado!
        scrapingSound = bikePhysicsController.bikeAudio.scrapingSound;
    }
    public void ResetTailData()
    {
        isBroken = false;
        tailResistance = 100;
        boxCollider.isTrigger = true;
    }
    void OnDestroy()
    {
        //StopAllCoroutines();
        stopScrapingAudio = null;
        tailStatus=null;
        disableParticleCoroutine = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isBroken || !gameManager.GetCurrentBikePhysicsController().isDoingWheelie)//Revisado!
            return;
        if (!scrapingSound.isPlaying)
            scrapingSound.Play();
        if (stopScrapingAudio != null)
        {
            StopCoroutine(stopScrapingAudio);
            stopScrapingAudio = null;
        }
        isScraping = true;
        tailResistance -= (100f / resistanceDuration) * Time.deltaTime;
        tailResistance = Mathf.Max(tailResistance, 0f);

        
        if (!collisionParticle.isPlaying)
            collisionParticle.Play();
        if (disableParticleCoroutine != null) 
        {
            StopCoroutine(disableParticleCoroutine);
            disableParticleCoroutine = null;
        }
        if (tailStatus == null)
            tailStatus = StartCoroutine(CheckTailStatus());
        if (transform.parent.gameObject.activeSelf)
            stopScrapingAudio = StartCoroutine(StopScrapingAudio());
    }
    private void OnTriggerStay(Collider other)
    {
        if (isBroken )
            return;
        if (stopScrapingAudio != null)
            StopCoroutine(StopScrapingAudio());
    }
    private void OnTriggerExit(Collider other)
    {
        if (isBroken)
            return;
        if (stopScrapingAudio == null)
            stopScrapingAudio = StartCoroutine(StopScrapingAudio());
        isScraping = false;
        tailStatus = null;
        //StopCoroutine(CheckTailStatus());
        if (disableParticleCoroutine != null)
        {
            StopCoroutine(disableParticleCoroutine);
            disableParticleCoroutine = null;
        }
        disableParticleCoroutine = StartCoroutine(DisableParticles());
        //Invoke(nameof(DisableParticles), 1);
    }
    IEnumerator CheckTailStatus()
    {
        while (!isBroken)
        {
            //Debug.Log("Checking...");
            if (tailResistance == 0)
            {
                StopAllCoroutines();
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
                    if(instance.GetComponent<BoxCollider>()!=null)
                        instance.GetComponent<BoxCollider>().enabled = true;
                    randomForce = Random.Range(12, 15);
                    force = Vector3.back * randomForce;
                    instance.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                    instance.transform.parent = null;
                    gameManager.SetDestruction(instance);
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
                gameManager.SetDestruction(instance);
                baseModel.SetActive(false);

                isBroken = true;
                tailStatus = null;
                isScraping = false;
                scrapingSound.Stop();
                gameManager.GetCurrentBikePhysicsController().DecreaseCondition(1);
            }
            yield return null;
        }
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
