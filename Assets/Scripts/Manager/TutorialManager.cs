using UnityEngine;
public class TutorialManager : MonoBehaviour
{
    public TutorialStep[] testTutorials = {
                                        new TutorialStep(0, 0, "Tutorial 1 / 7", "Next", 604, 450, 45, "This is your farm, you can also see its color on the top menu. It gives you 2GP per year as long as it's safe from the pest. If the pest reaches you, you lose the game."),
                                        new TutorialStep(640, 180, "Tutorial 2 / 7", "Next", 360, 170, 270, "This is the pest. If farmers don't do anything, it will spread each turn to one neighboring tile. To prevent the spreading, a collective of all the farmers have been formed."),
                                        new TutorialStep(400, 420, "Tutorial 3 / 7", "Next", 440, 160, 0, "Each farmer can contribute GP to the collective, and the more GP the collective gathers, the more efficient it will be to prevent the spread."),
                                        new TutorialStep(330, 420, "Tutorial 4 / 7", "Next", 300, 160, 0, "This is were you choose how much you want to contribute to the collective this year."),
                                        new TutorialStep(230, 420, "Tutorial 5 / 7", "Next", 100, 160, 0, "This is the amount of GP you have available."),
                                        new TutorialStep(240, 240, "Tutorial 6 / 7", "Next", 100, 480, 180, "This is the year counter. If you reach year 15, you win the game!"),
                                        new TutorialStep(240, 240, "Tutorial 7 / 8", "Next", 100, 480, 180, "In this box you will see information about the game as it plays, such as the amount of money collected by the collective, the result of the pest control... Keep an eye on it!"),
                                        new TutorialStep(302, 270, "Tutorial 7 / 7", "Start Test Game", "You will first play a test game with artificial players, always contributing the same amont. Feel free to experiment! Later, you will be connected with human players to play the real game.")
    };

    public TutorialStep[] studyTutorial = {
        new TutorialStep(240, 240, "Tutorial 1/2", "Next", "You will now be connected to four other players and play the study game.")
    };


    private int currentTutorial = 0;

    public TutorialStep[] tutorial;

    public void StartTestTutorial() 
    {
        currentTutorial = 0;
        tutorial = testTutorials;
        // displays the first tutorial
        Application.Instance.tutorialController.UpdateTutorialPanel(tutorial[currentTutorial]);

        // show the panel
        Application.Instance.tutorialController.ActivateTutorialPopup();
    }

    public void StartStudyTutorial()
    {
        currentTutorial = 0;
        tutorial = studyTutorial;
        // display the tutorial
        Application.Instance.tutorialController.UpdateTutorialPanel(tutorial[currentTutorial]);
        Application.Instance.tutorialController.ActivateTutorialPopup();
    }

    public void NextTutorialClicked() 
    {
        currentTutorial++;
        if(currentTutorial == tutorial.Length)
        {
            Application.Instance.tutorialController.DeactivateTutorialPopup();
            Application.Instance.protocolManager.TutorialFinished();
        }
        else 
        {
            Application.Instance.tutorialController.UpdateTutorialPanel(tutorial[currentTutorial]);
        }
    }

    public void DeactivateTutorial()
    {
        Application.Instance.tutorialController.DeactivateTutorialPopup();
    }
    
}