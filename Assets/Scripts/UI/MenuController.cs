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

    [SerializeField]
    private GameObject endGamePopup;

    [SerializeField]
    private Text endGamePopupText;


    [SerializeField]
    private GameObject connectionPopup;

    [SerializeField]
    private Text nbConnectedPlayers;

    [SerializeField]
    private GameObject codePopup;

    public bool endGame {get; set;}

    void Start()
    {
        Reset();
    }

    void Update()
    {
        if(Application.Instance.theWorld != null)
        {
            year.text = Application.Instance.theWorld.currentYear.ToString();
            walletText.text = Application.Instance.theWorld.humanPlayer.wallet.ToString();
        }
        
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

    public void ActivateEndGamePopup(string text)
    {
        endGamePopupText.text = text;
        endGamePopup.SetActive(true);
    }

    public void DeactivateEndGamePopup()
    {
        endGamePopup.SetActive(false);
    }

    public void ActivateConnectionPopup()
    {
        connectionPopup.SetActive(true);
    }

    public void DeactivateConnectionPopup()
    {
        connectionPopup.SetActive(false);
    }

    public void Reset()
    {
        // resets the UI
        popupDialog.SetActive(false);
        endGamePopup.SetActive(false);
        connectionPopup.SetActive(false);
        codePopup.SetActive(false);
        year.text = "0";
        contributionText.text = "0";

        ActivateMenu();

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

    public void EndGameOkClicked()
    {
        DeactivateEndGamePopup();
        Application.Instance.gameManager.EndGameClicked();
    }

    public void ActivateCodePopup()
    {
        codePopup.SetActive(true);
    }

}