using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeRim : MonoBehaviour
{
    Material baseMaterial;
    [SerializeField] bool isDummy;
    public void SetPartColor(Color color) =>
        GetComponent<MeshRenderer>().material.color = color;
    public Color GetRimColor()
    {
        return GetComponent<MeshRenderer>().material.color;
    }
    void Awake()
    {
        baseMaterial = GetComponent<MeshRenderer>().material;
        var material =  new Material(baseMaterial);
        GetComponent<MeshRenderer>().material = material;
    }

}
