using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidersController : MonoBehaviour
{
    [SerializeField] List<RiderData> riders;
    public List<RiderData> GetRidersData() { return riders; }
    public BikerAnimationController GetRider(int id)
    {
        foreach (var rider in riders)
        {
            if (rider.id == id)
            {
                return rider.mannequinPrefab.
                    GetComponent<BikerAnimationController>();
            }
        }
        return null;
    }
}
