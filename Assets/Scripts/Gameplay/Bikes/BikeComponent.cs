using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeBP_Pro;
public class BikeComponent : MonoBehaviour
{
    ArcadeBikeControllerPro bikeController;
    [SerializeField]List<BikeBody> bikeBodyPieces;
    public void SetBodyColor(Color color)
    {
        foreach (var bodyPiece in bikeBodyPieces)
            bodyPiece.SetPartColor(color);
    }
    [SerializeField]List<BikeRim> bikeRims;
    public void SetRimColor(Color color)
    {
        foreach(var rim in bikeRims)
            rim.SetPartColor(color);
    }
    private void Awake()=>
        bikeController = GetComponent<ArcadeBikeControllerPro>();
    public void SetTailSectionFix() =>
        StartCoroutine(FixTailSection(1));
    IEnumerator FixTailSection(float fadeDuration)
    {
        var tailElements = new List<GameObject>();
        var tailSection = bikeController.bikeReferences.tailSection;
        tailSection.baseModel.SetActive(true);
        tailElements.Add(tailSection.baseModel);
        if (tailSection.additionalPieces.Count > 0)
        {
            foreach (var piece in tailSection.additionalPieces)
            {
                tailElements.Add(piece.gameObject);
                SetObjectAlpha(piece.GetComponent<Renderer>(), 0);
            }
        }

        foreach (var element in tailElements)
        {
            var currentRenderer = element.GetComponent<Renderer>();
            foreach (var mat in currentRenderer.materials)
                SetMaterialToFade(mat);

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                SetObjectAlpha(currentRenderer, alpha);
                yield return null;
            }

            SetObjectAlpha(currentRenderer ,1f);

            foreach (Material mat in currentRenderer.materials)
                SetMaterialToOpaque(mat);
        }
    }

    public void EnterBike()
    {
        Debug.Log("Instantiate the bike!");
    }
    private void SetObjectAlpha(Renderer renderer, float alpha)
    {
        foreach (Material mat in renderer.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }
    }

    private void SetMaterialToFade(Material mat)
    {
        mat.SetFloat("_Mode", 2);  // Set render mode to Fade
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    private void SetMaterialToOpaque(Material mat)
    {
        mat.SetFloat("_Mode", 0);  // Set render mode to Opaque
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }
}
