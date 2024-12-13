using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeBP_Pro;
using System.IO;
using Unity.VisualScripting;
public class BikeComponent : MonoBehaviour
{
    [SerializeField]List<BikeBody> colorableBodyParts;
    List<GameObject> detachableParts;
    public List<GameObject> models { get; private set; }
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
    [SerializeField] bool isModelOnly;
    private void Awake()
    {
        detachableParts = new List<GameObject>();
        if(tailSection!=null)
            detachableParts.Add(tailSection);
        if(leftMirror)
            detachableParts.Add(leftMirror);
        if (rightMirror)
            detachableParts.Add(rightMirror);
        if (isModelOnly)
        {
            models = new List<GameObject>();
            foreach (Transform child in transform.GetChild(0))
                models.Add(child.gameObject);
        }
    }
    public void TriggerDetachablesPartsFix() =>
        StartCoroutine(FadeDetachablePartsIn(1));
    IEnumerator FadeDetachablePartsIn(float fadeDuration)
    {
        var fadeableParts = new List<GameObject>();
        foreach (var detachablePart in detachableParts)
        {

            if (detachablePart.activeSelf)
                continue;
            fadeableParts.Add(detachablePart);
            //SetObjectAlpha(fadeableParts[0].GetComponent<MeshRenderer>(), 0);
            if (detachablePart.transform.GetChild(0).GetComponent<TailSection>() != null)
            {
                var tailSection = detachablePart.transform.
                    GetChild(0).GetComponent<TailSection>();
                tailSection.ResetTailData();
                if (tailSection.additionalPieces.Count > 0)
                {
                    foreach (var piece in tailSection.additionalPieces)
                    {
                        if (piece.GetComponent<MeshRenderer>() == null)
                            continue;
                        fadeableParts.Add(piece.gameObject);
                        //SetObjectAlpha(piece.GetComponent<MeshRenderer>(), 0);
                        //piece.gameObject.SetActive(true);
                    }
                }
                //detachablePart.gameObject.SetActive(true);
            }
            foreach (var fadeablePart in fadeableParts)
            {
                var currentRenderer = fadeablePart.GetComponent<MeshRenderer>();
                foreach (var mat in currentRenderer.materials)
                    SetFadeableMaterial(mat);
                fadeablePart.SetActive(true);

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
                mat.SetFloat("_Mode", 2);
            }
        }
    }

    private void SetFadeableMaterial(Material mat)
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
