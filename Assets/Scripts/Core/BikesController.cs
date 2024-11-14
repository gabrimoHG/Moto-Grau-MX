using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikesController : MonoBehaviour
{
    [SerializeField]List<BikeData> bikes;
    public List<BikeData> GetBikes() { return bikes; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
