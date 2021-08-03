using UnityEngine;
using UnityEngine.UI;
using System;
public class MenuController : MonoBehaviour
{
    [SerializeField]
    private Text year;

    [SerializeField]   
    private GameObject popupDialog;

    [SerializeField]
    private Text popupDialogText;

    [SerializeField]
    private Text walletText;

    [SerializeField]
    private Text contributionText;

    [SerializeField]
    private Button upButton;
    
    [SerializeField]
    private Button downButton;

    [SerializeField]
    private Button payButton;


    public bool endGame {get; set;}

    void Start()
    {
        popupDialog.SetActive(false);
    }

    void Update()
    {
        walletText.text = Application.Instance.theWorld.humanPlayer.wallet.ToString();
    }

    public void NextYear()
    {
        year.text = Application.Instance.theWorld.currentYear.ToString();
    }

    public void ActivateMenu()
    {
        upButton.interactable = true;
        downButton.interactable = true;
        payButton.interactable = true;
    }

    public void DeactivateMenu()
    {
        upButton.interactable = false;
        downButton.interactable = false;
        payButton.interactable = false;
    }

    public int ProcessContribution()
    {
        int contribution = int.Parse(contributionText.text);
        int newWallet = int.Parse(walletText.text) - contribution;
        walletText.text = newWallet.ToString();
        contributionText.text = "0";
        return contribution;
    }

    public void ActivatePopup(string text)
    {
        popupDialogText.text = text;
        popupDialog.SetActive(true);
    }

    public void DeactivatePopup()
    {
        popupDialog.SetActive(false);
    }

    // methods called when clicking on the buttons
    public void IncreaseAmount()
    {
        int newContribution = Math.Min(int.Parse(contributionText.text) + 1, int.Parse(walletText.text));
        contributionText.text = newContribution.ToString();
    }

    public void DecreaseAmount()
    {
        int newContribution = Math.Max(int.Parse(contributionText.text) - 1, 0);
        contributionText.text = newContribution.ToString();
    }

    public void Pay()
    {
        Debug.Log("MenuController.Pay");
        Application.Instance.gameManager.Paid();
    }

}