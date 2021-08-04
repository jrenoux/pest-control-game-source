using UnityEngine;
using UnityEngine.UI;
public class TutorialController : MonoBehaviour 
{
    private bool isTutorialActive = false;
    [SerializeField]
    private GameObject tutorialPopup;
    [SerializeField]
    private Image arrowImage;
    

    public void Start() 
    {
        tutorialPopup.SetActive(false);
        isTutorialActive = false;
    }

    public void Update()
    {
    
    }

    public void ActivateTutorialPopup()
    {
        isTutorialActive = true;
        tutorialPopup.SetActive(true);
    } 

    public void DeactivateTutorialPopup()
    {
        isTutorialActive = false;
        tutorialPopup.SetActive(false);
    }

    public void NextButtonClicked()
    {
        Application.Instance.tutorialManager.NextTutorialClicked();
    }

    public void DisplayNextTutorial()
    {
    }

    public void UpdateTutorialPanel(TutorialStep tutorial)
    {
        tutorialPopup.transform.position = tutorial.PopupPositionDelta;
        Text[] texts = tutorialPopup.GetComponentsInChildren<Text>();
        Text tutorialTitle = texts[0];
        Text tutorialMessage = texts[1];
        Text tutorialButtonText = texts[2];
        tutorialTitle.text = tutorial.Title;
        tutorialMessage.text = tutorial.Message;
        tutorialButtonText.text = tutorial.ButtonText;
        if(tutorial.ArrowPositionDelta != null && tutorial.ArrowRotationDelta != null)
        {
            arrowImage.transform.position = tutorial.ArrowPositionDelta.Value;
            arrowImage.transform.rotation = tutorial.ArrowRotationDelta.Value;    
        }
        else
        {
            arrowImage.gameObject.SetActive(false);
        }
        
    }

    
}