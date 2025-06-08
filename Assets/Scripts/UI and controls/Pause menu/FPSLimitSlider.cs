using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSLimitSlider : MonoBehaviour
{
    GameManager gameManager;
    public Slider slider { protected set; get; }
    public TextMeshProUGUI currentSelectionUI { protected set; get; }
    // Start is called before the first frame update
    protected void Awake()
    {
        slider = GetComponent<Slider>();
        currentSelectionUI = transform.GetChild(4).
            GetComponent<TextMeshProUGUI>();
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("FPSLimit"))
            slider.value = PlayerPrefs.GetInt("FPSLimit");
    }
    void OnEnable()
    {
        UpdateCurrentSelection(slider.value);
        slider.onValueChanged.AddListener(gameManager.SetFPSLimit);
        slider.onValueChanged.AddListener(UpdateCurrentSelection);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(gameManager.SetFPSLimit);
        slider.onValueChanged.RemoveListener(UpdateCurrentSelection);
    }
    public int ConvertedFPSValue()
    {
        switch (slider.value)
        {
            case 0:
                return 30;
            case 1:
                return 60;
            case 2:
                return 300;
            default:
                break;
        }
        return 30;
    }
    void UpdateCurrentSelection(float fpsLimitIndex)
    {
        if (fpsLimitIndex == 0)
            currentSelectionUI.text = "30";
        else if (fpsLimitIndex == 1)
            currentSelectionUI.text = "60";
        else if (fpsLimitIndex == 2)
            currentSelectionUI.text = "Sem limite";
    }
}
