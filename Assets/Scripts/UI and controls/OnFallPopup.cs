using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnFallPopup : MonoBehaviour
{
    Button resetBike;
    // Start is called before the first frame update
    void Awake()
    {
        resetBike = transform.GetChild(0).GetComponent<Button>();
        resetBike.onClick.AddListener(GameManager.instance.ResetBike);
        resetBike.onClick.AddListener(DisableContainer);
    }
    void DisableContainer() => gameObject.SetActive(false);
}
