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
    private GameObject wallet;

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

    [SerializeField]
    private Image playerColorToken;

    [SerializeField] 
    private Camera camera;

    public bool endGame {get; set;}

    void Start()
    {
        Reset();
    }

    void Update()
    {
        if(PestApplication.Instance.theWorld != null)
        {
            year.text = PestApplication.Instance.theWorld.currentYear.ToString();
            walletText.text = PestApplication.Instance.theWorld.humanPlayer.wallet.ToString();
            string playerColor = PestApplication.Instance.theWorld.humanPlayer.id;
            playerColorToken.sprite = Resources.Load<Sprite>("Sprites/circle_" + playerColor);
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

    public int GetCurrentContribution()
    {
        return int.Parse(contributionText.text);   
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
        PestApplication.Instance.gameManager.Paid();
    }

    public void EndGameOkClicked()
    {
        DeactivateEndGamePopup();
        PestApplication.Instance.gameManager.EndGameClicked();
    }

    public void ActivateCodePopup()
    {
        codePopup.SetActive(true);
    }

    public GameObject GetWallet()
    {
        return wallet;
    }

    public Vector3 GetWalletPosition()
    {
        Debug.Log(wallet.GetComponent<RectTransform>().position);
        return GUIUtility.GUIToScreenPoint(wallet.GetComponent<RectTransform>().position);
    }

    public Rect GetWalletRect()
    {
        return wallet.GetComponent<RectTransform>().rect;
    }

}