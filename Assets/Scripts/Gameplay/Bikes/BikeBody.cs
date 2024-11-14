using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeBody : MonoBehaviour
{
    Material baseMaterial;
    [SerializeField] int specificMaterialIndex;
    [SerializeField] bool isDummy;
    public Color GetBodyColor()
    {
        return  GetComponent<MeshRenderer>().materials[specificMaterialIndex].color;
    }
    public void SetPartColor(Color color) => 
        GetComponent<MeshRenderer>().materials[specificMaterialIndex].color = color;
    void Awake()
    {
        baseMaterial = GetComponent<MeshRenderer>().materials[specificMaterialIndex];
        var materialInstance = new Material(baseMaterial);
        GetComponent<MeshRenderer>().materials[specificMaterialIndex] = materialInstance;
    }

}
