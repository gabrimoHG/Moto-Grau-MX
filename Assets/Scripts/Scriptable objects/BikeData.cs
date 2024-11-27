using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New bike", menuName = "ScriptableObjects/Bike data", order = 1)]
public class BikeData : GameData
{
    public GameObject mainPrefab;
    public GameObject dummyPrefab;
    public Sprite silhouette;
}
