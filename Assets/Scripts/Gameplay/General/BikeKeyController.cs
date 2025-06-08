using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeKeyController : CollectibleController
{
    [SerializeField]GameObject bikeKeyPrefab;
    protected override void Start()
    {

    }

    protected override void SetInternalData()
    {
        var isKeyCollectedInt = PlayerPrefs.GetInt("IsNextBikeKeyCollected");
        if (isKeyCollectedInt == 1)
            return;
        int randomIndex = Random.Range(0, transform.childCount);
        Transform chosenChild = transform.GetChild(randomIndex);

        // Instantiate the prefab as a child of the chosen child
        GameObject newObject = Instantiate(bikeKeyPrefab, chosenChild.position,
            Quaternion.identity, chosenChild);

    }
}
