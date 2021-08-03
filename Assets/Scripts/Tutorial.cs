using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialPopup;
    private TutorialStep[] tutorials = {
                                        new TutorialStep(230, 370, "Tutorial 1 / 6", "Next", 500, 350, 90, "This is your farm, it gives you 2GP per year as long as it's safe from the pest. If the pest reaches you, you lose the game."),
                                        new TutorialStep(640, 180, "Tutorial 2 / 6", "Next", 360, 170, 270, "This is the pest. If farmers don't do anything, it will spread each turn to one neighboring tile. To prevent the spreading, a collective of all the farmers have been formed."),
                                        new TutorialStep(400, 420, "Tutorial 3 / 6", "Next", 440, 160, 0, "Each farmer can contribute GP to the collective, and the more GP the collective gathers, the more efficient it will be to prevent the spread."),
                                        new TutorialStep(330, 420, "Tutorial 4 / 6", "Next", 300, 160, 0, "This is were you choose how much you want to contribute to the collective this year."),
                                        new TutorialStep(230, 420, "Tutorial 5 / 6", "Next", 100, 160, 0, "This is the amount of GP you have available."),
                                        new TutorialStep(240, 240, "Tutorial 6 / 6", "Start Game", 100, 480, 180, "This is the year counter. If you reach year 15, you win the game!")
    };
    private int currentTutorial = 0;

    private bool isTutorialActive = false;


    public void Start()
    {
        tutorialPopup.SetActive(false);
        isTutorialActive = false;
    }

    public void Update()
    {
        if(isTutorialActive)
        {
            UpdateTutorialPanel();
        }
    }

    public void StartTutorial() 
    {
        tutorialPopup.SetActive(true);
        isTutorialActive = true;
    }

    private void UpdateTutorialPanel()
    {
        tutorialPopup.transform.position = tutorials[currentTutorial].PopupPositionDelta;
        Text[] texts = tutorialPopup.GetComponentsInChildren<Text>();
        Text tutorialTitle = texts[0];
        Text tutorialMessage = texts[1];
        Text tutorialButtonText = texts[2];
        tutorialTitle.text = tutorials[currentTutorial].Title;
        tutorialMessage.text = tutorials[currentTutorial].Message;
        tutorialButtonText.text = tutorials[currentTutorial].ButtonText;
        Image[] images = tutorialPopup.GetComponentsInChildren<Image>();
        Image arrow = images[1];
        arrow.transform.position = tutorials[currentTutorial].ArrowPositionDelta;
        arrow.transform.rotation = tutorials[currentTutorial].ArrowRotationDelta;
    }

    // Called by a press on the "Next" Button
    public void NextTutorial()
    {
        currentTutorial++;
        if (currentTutorial == tutorials.Length)
        {
            //end of tutorial andstart game
            tutorialPopup.SetActive(false);
            // tell the protocol that the tutorial is finished
            // TODO
        }
        else
        {
            UpdateTutorialPanel();
        }
    }
}