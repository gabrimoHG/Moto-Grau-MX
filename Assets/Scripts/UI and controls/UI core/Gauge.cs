using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ArcadeBP_Pro;
using UnityEngine.UI;
using UnityEngine.Rendering.UI;

public class Gauge : MonoBehaviour
{
    protected GameManager gameManager;
    protected ArcadeBikeControllerPro bikePhysicsController;
    protected BikeAdditionalPhysics bikeAdditionalPhysics;
    protected TextMeshProUGUI TMProContent;
    protected Image imageContent;
    protected Color lowColor, highColor;
    [SerializeField] private float fadeDuration = 0.5f; // Time to fade in/out
    [SerializeField] private float delayBetweenFades = 0.2f; // Delay before the next fade cycle
    protected Coroutine fadeLoop;
    protected virtual void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();   
        TMProContent = GetComponent<TextMeshProUGUI>();
        imageContent = GetComponent<Image>();
        lowColor = Color.red;
        highColor = Color.green;
    }
    protected virtual void Start()
    {
        bikePhysicsController = FindObjectOfType<GameManager>().GetCurrentBikePhysicsController();
        bikeAdditionalPhysics = bikePhysicsController.GetComponent<BikeAdditionalPhysics>();
        FindObjectOfType<GameManager>().onPlayerBikeChange += UpdateBikeController;//Revisado!
                                                                                   //GameManager.instance.onBikeReset += UpdateBikeController;
                                                                                   //Debug.Log($"{name} - Gauge start called at frame {Time.frameCount}");
    }

    public void UpdateBikeController(ArcadeBikeControllerPro bikePhysicsController) =>
        this.bikePhysicsController = bikePhysicsController;

    /*public void UpdateBikeController() =>
        bikePhysicsController = GameManager.instance.GetCurrentBikePhysicsController();*/

    protected IEnumerator FadeLoop()
    {
        while (true)
        {
            yield return StartCoroutine(Fade(0f, 1f));

            yield return new WaitForSeconds(delayBetweenFades);

            yield return StartCoroutine(Fade(1f, 0f));

            yield return new WaitForSeconds(delayBetweenFades);

            Debug.Log("Fade loop!");
        }

    }

    protected IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        var spriteColor = Color.red;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            spriteColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            imageContent.color = spriteColor;

            yield return null;
        }

        // Ensure final alpha is set
        spriteColor.a = endAlpha;
        imageContent.color = spriteColor;
    }
    protected virtual void StopFading()
    {
        if (fadeLoop != null)
        {
            StopCoroutine(fadeLoop);
            fadeLoop = null;
            //Debug.Log("Stop fade loop!");
        }

        /*Color spriteColor = imageContent.color;
        spriteColor.a = 1f;
        imageContent.color = spriteColor;*/
        //Debug.Log("Parou de piscar!");
    }
}
