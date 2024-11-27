using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiderComponent : MonoBehaviour
{
    [SerializeField] RiderData riderData;
    public RiderData RiderData { get { return riderData; } }
}
