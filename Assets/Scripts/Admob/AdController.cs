using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdController : MonoBehaviour
{
    [SerializeField]protected float showCooldown;
    [SerializeField]protected string adTypeClickDate;
    public bool isAdClicked { protected set; get; }
    public bool isAvailable { protected set; get; }
    protected string currentDate, adClickDate, simulatedData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected IEnumerator ShowAdCooldown()
    {
        isAvailable = false;

        float cooldownEndTime = Time.time + showCooldown;
        while (Time.time < cooldownEndTime)
        {
            float remainingTime = cooldownEndTime - Time.time;
            yield return new WaitForSeconds(1f); // Check every second
        }

        isAvailable = true;
    }

    protected virtual IEnumerator SetIsAdClicked()
    { 
        while (isAdClicked)
        {
            currentDate = System.DateTime.Now.ToString("yyyyMMdd");
            if (string.IsNullOrEmpty(simulatedData))
                adClickDate = PlayerPrefs.GetString(adTypeClickDate);
            else
                adClickDate = simulatedData;

            if (currentDate != adClickDate)
            {
                isAdClicked = false;
                simulatedData = "";
            }
            yield return new WaitForSeconds(1f);
        }
    }

    protected void SetAdClickRegister()
    {
        currentDate = System.DateTime.Now.ToString("yyyyMMdd");
        PlayerPrefs.SetString(adTypeClickDate, currentDate);
        isAdClicked = true;
    }
    public void OverrideLastPlayedData()
    {
        simulatedData = System.DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
        PlayerPrefs.SetString(adTypeClickDate, simulatedData);
    }
}
