using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New bike", menuName = "ScriptableObjects/Bike", order = 1)]
public class BikeData : GameData
{
    public GameObject mainPrefab;
    public GameObject dummyPrefab;
}
