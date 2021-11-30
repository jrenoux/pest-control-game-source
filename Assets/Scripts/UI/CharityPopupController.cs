using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class CharityPopupController : MonoBehaviour
{
    [SerializeField]
    private Text popupText;

    [SerializeField]
    private InputField charityDonation;

    [SerializeField]
    private GameObject sliderPopup;

    [SerializeField]
    private Button confirmButton;

    [SerializeField]
    private GameObject charityToggleComponent;

    private double convertionFactor = 0.02;

    private String[] imagesToDisplay = {"unicef", "eacr", "efb"};
    private String[] charityNames = {"Unicef", "European Association for Cancer Research", "European Food Banks Association"};
    List<GameObject> logoList;


    // Start is called before the first frame update
    void Start()
    {
        DeactivateSliderPopup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateSliderPopup()
    {
        // get coins in wallet
        int coins = PestApplication.Instance.theWorld.humanPlayer.wallet;
        // convert to bonus
        double bonus = coins * convertionFactor;
        // replace in the text
        popupText.text = popupText.text.Replace("<coins>", coins.ToString()).Replace("<bonus>", bonus.ToString());

        // deactivate the ok button until the value of the text is changed
        confirmButton.interactable = false;

        // TODO place the three charity
        // get the list of images
        RawImage[] charityImages = charityToggleComponent.GetComponentsInChildren<RawImage>();

        // get the list of toggles
        Toggle[] toggles = charityToggleComponent.GetComponentsInChildren<Toggle>();

        ToggleGroup togglegroup = charityToggleComponent.GetComponentInChildren<ToggleGroup>();
        Debug.Log(togglegroup.name);
        // register all toggles to togglegroup
        foreach(Toggle t in toggles) {
            togglegroup.RegisterToggle(t);

        }

        // ensure that the number of images and toggles is equal to the number of images to display
        if(charityImages.Length != toggles.Length || charityImages.Length != imagesToDisplay.Length || charityImages.Length != charityNames.Length) {
            Debug.LogError("The number of charities to display is different from the number of available slots");
        }
        else {
            // we display
            var rand = new System.Random();
            // get a random logo to select
            int rInt = rand.Next(0, imagesToDisplay.Length - 1);
            toggles[rInt].isOn = true;
            Debug.Log("Selected toggle " + toggles[rInt] + ", named " + toggles[rInt].name);

            // create a range of indices to browse in random order
            IEnumerable<int> indices = Enumerable.Range(0, imagesToDisplay.Length);
            Debug.Log(indices);
            int containerId = 0;
            foreach(int id in indices.OrderBy(x => rand.Next())) {
                Texture2D texture = Resources.Load("RawImages/" + imagesToDisplay[id]) as Texture2D;
                charityImages[containerId].texture = texture;
                toggles[containerId].GetComponentInChildren<Text>().text = charityNames[id];
                containerId = containerId + 1;
            }
                            
        }

        sliderPopup.SetActive(true);
    }

    public void DeactivateSliderPopup()
    {
        sliderPopup.SetActive(false);
    }

    public void Confirm()
    {
        // we tell the protocol manager that the player donated the value in charityValue
        PestApplication.Instance.protocolManager.Donate(float.Parse(charityDonation.text));
        Debug.Log("The player donated " + charityDonation.text);
        DeactivateSliderPopup();
    }

    public void OnTextFieldChanged()
    {
        // ensure that the text is only numerical
        // activate the button if it is, deactivate otherwise
        //var regex = "^[0-9]+$";
        var regex = @"^[0-9]+(?:\.[0-9]*)?$";

        var match = Regex.Match(charityDonation.text, regex);



        if(match.Success)
        {
            // confirms that the amount is lower than the bonus
            // get coins in wallet
            int coins = PestApplication.Instance.theWorld.humanPlayer.wallet;
            // convert to bonus
            double bonus = coins * convertionFactor;

            double donation = double.Parse(charityDonation.text);
            if (donation <= bonus) {
                confirmButton.interactable = true;
            }
            else {
                confirmButton.interactable = false;
            }
            
        }
        else 
        {
            confirmButton.interactable = false;
        }
    }

    public void OnClickOnCharity()
    {
        // change the logo selected
        
    }
}
