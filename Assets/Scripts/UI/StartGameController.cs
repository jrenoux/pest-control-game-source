using UnityEngine;
using UnityEngine.UI;

public class StartGameController : MonoBehaviour
{
    [SerializeField]
    private GameObject initOverlay;

    [SerializeField]
    private InputField prolificInputField;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private GameObject prolificSection;

    public void Update()
    {

    }
    public void DisplayInitOverlay()
    {
        initOverlay.SetActive(true);
        if(PestApplication.Instance.prolificID.Equals(""))
        {
            // we didn't get the id, we need the field
            prolificSection.gameObject.SetActive(true);

            // button deactivated
            startButton.interactable = false;
        }
        else 
        {
            prolificSection.gameObject.SetActive(false);
            startButton.interactable = true;
        }
        
    }

    public void OnProlificFieldEdited()
    {
        bool isValid = false;
        string prolificID = prolificInputField.text;
        // test that the prolific ID is valid
        if (PestApplication.Instance.ValidateProlificID(prolificID))
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


        // get the prolific ID from the text field 
        string prolificID = prolificInputField.text;
         
        // inform the protocol manager
        PestApplication.Instance.protocolManager.StartTutorialClicked(prolificID);

    }
}