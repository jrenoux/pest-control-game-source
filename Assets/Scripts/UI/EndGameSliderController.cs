using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EndGameSliderController : MonoBehaviour
{
    [SerializeField]
    private Text popupText;

    [SerializeField]
    private Text charityValue;

    [SerializeField]
    private GameObject sliderPopup;

    private double convertionFactor = 0.1;

    // Start is called before the first frame update
    void Start()
    {
        DeactivateSliderPopup();
        charityValue.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateCharityValue(float value)
    {
        // multiply the current wallet amount by the slider value
        // rounded up
        double charityContribution = Math.Round(PestApplication.Instance.theWorld.humanPlayer.wallet * convertionFactor * value, 2);
        
        charityValue.text = charityContribution.ToString();
    }

    public void ActivateSliderPopup()
    {
        // get coins in wallet
        int coins = PestApplication.Instance.theWorld.humanPlayer.wallet;
        // convert to bonus
        double bonus = coins * convertionFactor;
        // replace in the text
        popupText.text = popupText.text.Replace("<coins>", coins.ToString()).Replace("<bonus>", bonus.ToString());
        sliderPopup.SetActive(true);
    }

    public void DeactivateSliderPopup()
    {
        sliderPopup.SetActive(false);
    }

    public void Donate()
    {
        // we tell the protocol manager that the player donated the value in charityValue
        PestApplication.Instance.protocolManager.Donate(float.Parse(charityValue.text));
        Debug.Log("The player donated " + charityValue.text);
        DeactivateSliderPopup();
    }

    public void DontDonate()
    {   
        // we tell the protocol manager that the player donated 0
        PestApplication.Instance.protocolManager.Donate(0);
        Debug.Log("The player did not donate");
        DeactivateSliderPopup();
    }
}
