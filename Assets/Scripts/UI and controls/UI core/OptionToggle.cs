using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionToggle : MonoBehaviour
{
    protected GameManager gameManager;
    protected Toggle toggle;

    protected virtual void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        toggle = GetComponent<Toggle>();
    }
}
