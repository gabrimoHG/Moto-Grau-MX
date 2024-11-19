using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeBP_Pro;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Windows.Speech;
using System.IO;
using Unity.VisualScripting;
public class BikeComponent : MonoBehaviour
{
    ArcadeBikeControllerPro bikeController;
    [SerializeField]List<BikeBody> colorableBodyParts;
    List<GameObject> detachableParts;
    [SerializeField] GameObject tailSection, leftMirror, rightMirror;
    public bool IsTailActive() { return tailSection.activeSelf; }
    public void SetIsTailActive(bool isActive)=>tailSection.SetActive(isActive);
    public bool IsLeftMirrorActive() {  return leftMirror.activeSelf; }
    public void SetIsLeftMirrorActive(bool isActive) => leftMirror.SetActive(isActive);
    public bool IsRightMirrorActive() { return rightMirror.activeSelf; }
    public void SetIsRightMirrorActive(bool isActive) => rightMirror.SetActive(isActive);
    public List<BikeBody> GetBikeColorableBodyPieces() { return colorableBodyParts; }
    public Color GetBodyColor() { return colorableBodyParts[0].GetBodyColor(); }
    public void SetBodyColor(Color color)
    {
        foreach (var bodyPiece in colorableBodyParts)
            bodyPiece.SetPartColor(color);
    }
    [SerializeField]List<BikeRim> bikeRims;
    public List<BikeRim> GetBikeRims() { return bikeRims; }
    public Color GetRimsColor() { return bikeRims[0].GetRimColor(); }
    public void SetRimsColor(Color color)
    {
        foreach(var rim in bikeRims)
            rim.SetPartColor(color);
    }
    private void Awake()
    {
        bikeController = GetComponent<ArcadeBikeControllerPro>();
        detachableParts = new List<GameObject>();
        if(tailSection!=null)
            detachableParts.Add(tailSection);
        if(leftMirror)
            detachableParts.Add(leftMirror);
        if (rightMirror)
            detachableParts.Add(rightMirror);
    }
    public void SetDetachablesPartsFix() =>
        StartCoroutine(FadeDetachablePartsIn(1));
    IEnumerator FadeDetachablePartsIn(float fadeDuration)
    {
        var fadeableParts = new List<GameObject>();
        foreach (var detachablePart in detachableParts)
        {

            if (detachablePart.activeSelf)
                continue;
            fadeableParts.Add(detachablePart);
            if (detachablePart.transform.GetChild(0).GetComponent<TailSection>() != null)
            {
                var tailSection = detachablePart.transform.
                    GetChild(0).GetComponent<TailSection>();
                if (tailSection.additionalPieces.Count > 0)
                {
                    foreach (var piece in tailSection.additionalPieces)
                    {
                        fadeableParts.Add(piece.gameObject);
                        SetObjectAlpha(piece.GetComponent<Renderer>(), 0);
                    }
                }
            }

            foreach (var element in fadeableParts)
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

                SetObjectAlpha(currentRenderer, 1f);

                foreach (Material mat in currentRenderer.materials)
                    SetMaterialToOpaque(mat);
            }
        }
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
