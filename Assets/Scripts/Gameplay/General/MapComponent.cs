using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapComponent : MonoBehaviour
{
    [SerializeField] MapData mapData;
    public MapData MapData { get { return mapData; } }
}
