using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
public class StartGameController : MonoBehaviour
{
    [SerializeField]
    private GameObject initOverlay;

    [SerializeField]
    private InputField prolificInputField;

    [SerializeField]
    private Button startButton;

    public void Update()
    {

    }
    public void DisplayInitOverlay()
    {
        initOverlay.SetActive(true);
        // button deactivated
        startButton.interactable = false;

        // activate the button when the prolific number has been entered

    }

    public void OnProlificFieldEdited()
    {
        bool isValid = false;
        string prolificID = prolificInputField.text;
        // test that the prolific ID is valid
        // prolific IDs contain 24 alphanumerical characters 
        var regex = @"^\w{24}$";

        var match = Regex.Match(prolificID, regex, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;         
        }
        
        
    }

    public void StartTutorialClicked() 
    {
        // revome the init overlay
        initOverlay.SetActive(false);

        // get the prolific ID
        string prolificID = prolificInputField.text;
        

        // inform the protocol manager
        PestApplication.Instance.protocolManager.StartTutorialClicked(prolificID);

    }
}