using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GraphicsQualitySlider : MonoBehaviour
{
    public Slider slider { protected set; get; }
    public TextMeshProUGUI currentSelectionUI { protected set; get; }
    // Start is called before the first frame update
    protected void Awake()
    {
        slider = GetComponent<Slider>();
        currentSelectionUI = transform.GetChild(4).
            GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("GraphicQuality"))
            slider.value = (int)PlayerPrefs.GetFloat("GraphicQuality");
    }
    void OnEnable()
    {
        UpdateCurrentSelection(slider.value);
        //slider.onValueChanged.AddListener(GameManager.instance.SetGraphicQuality);
        slider.onValueChanged.AddListener(UpdateCurrentSelection);
    }

    void OnDisable()
    {
        //slider.onValueChanged.RemoveListener(GameManager.instance.SetGraphicQuality);
        slider.onValueChanged.RemoveListener(UpdateCurrentSelection);
    }

    void UpdateCurrentSelection(float qualityIndex)
    {
        if (qualityIndex == 0)
            currentSelectionUI.text = "Muita baixa";
        else if (qualityIndex == 1)
            currentSelectionUI.text = "Baixa";
        else if (qualityIndex == 2)
            currentSelectionUI.text = "Normal";
        else if (qualityIndex == 3)
            currentSelectionUI.text = "Alta";
        else if (qualityIndex == 4)
            currentSelectionUI.text = "Muito alta";
        else if (qualityIndex == 5)
            currentSelectionUI.text = "Ultra";
    }
}
