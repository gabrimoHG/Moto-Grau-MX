using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MainMenuButton : MonoBehaviour
{
    protected Button button;
    protected string message;
    protected WarningPopup warningPopup;
    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        warningPopup = MainMenuManager.instance.warningPopup;
    }
    public abstract void OnClick();
}
