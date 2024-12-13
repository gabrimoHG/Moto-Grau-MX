using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetBikeButton : MonoBehaviour
{
    Button button;
    private void Awake()=>
        button = GetComponent<Button>();
    private void OnEnable()
    {
        button.onClick.RemoveAllListeners();
        var bikePhysicsController = FindObjectOfType<GameManager>().GetCurrentBikePhysicsController();
        button.onClick.AddListener(bikePhysicsController.ResetToLastValidPosition);
    }
}
