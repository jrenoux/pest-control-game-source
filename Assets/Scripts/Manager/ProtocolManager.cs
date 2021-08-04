using UnityEngine;
public enum ProtocolStates 
{
    Start, // display the init overlay that wait for the player to click "Start" after entering the Prolific ID
    TestTutorial, // display the tutorial
    TestGame, // start a fully random test game
    StudyTutorial,
    StudyGame, // start the scripted game, that is going to be analyzed
    Questionnaire, // display the link to the questionnaires
}
public class ProtocolManager : MonoBehaviour
{
    private ProtocolStates currentState = ProtocolStates.Start;
    private bool hasStateChanged = false;

    public void Update()
    {
        if(hasStateChanged)
        {
            switch(currentState)
            {
                case ProtocolStates.Start:
                // nothing to do
                break;

                case ProtocolStates.TestTutorial:
                StartTestTutorial();
                break;

                case ProtocolStates.TestGame:
                StartTestGame();
                break;

                case ProtocolStates.StudyTutorial:
                StartStudyTutorial();
                break;

                case ProtocolStates.StudyGame:
                StartStudyGame();
                break;

                case ProtocolStates.Questionnaire:
                StartQuestionnaire();
                break;
            }
            hasStateChanged = false;
        }
    }

    public void Start() 
    {
        // starts the protocol, make sure things are in place
        // display the overlay
        Application.Instance.startGameController.DisplayInitOverlay();
        // deactivate the tutorial
        Application.Instance.tutorialManager.DeactivateTutorial();
    }

    ////////////////////////////////////////////////////State Machine Functions
    public void StartTestTutorial() 
    {
        // setting up the game
        Application.Instance.SetupTestGame();
        // starting the tutorial
        Application.Instance.tutorialManager.StartTestTutorial();
    }
     
    public void StartTestGame()
    {
        Debug.Log("Start Test Game");
        Application.Instance.gameManager.StartGame(true);
    }

    public void StartStudyTutorial() 
    {
        Application.Instance.SetupStudyGame();
        Application.Instance.tutorialManager.StartStudyTutorial();
    }

    public void StartStudyGame()
    {
        Debug.Log("Start Study Game");
        Application.Instance.gameManager.StartGame(false);
    }

    public void StartQuestionnaire() 
    {
        Debug.Log("Start Questionnaires");
        Application.Instance.menuController.ActivateCodePopup();
    }

    private void SetState(ProtocolStates state)
    {
        hasStateChanged = true;
        currentState = state;
    }

    ////////////////////////////////////////////////////Action functions
    public void StartTutorialClicked(string prolificID) 
    {
        // store the prolificID
        Application.Instance.prolificID = prolificID;
        SetState(ProtocolStates.TestTutorial);
    }

    
    public void TutorialFinished()
    {
        if(currentState == ProtocolStates.TestTutorial)
        {
            SetState(ProtocolStates.TestGame);
        }
        else if(currentState == ProtocolStates.StudyTutorial)
        {
            SetState(ProtocolStates.StudyGame);
        }
        
    }

    public void GameFinished()
    {
        // todo
        Debug.Log("Game Finished");
        if(currentState == ProtocolStates.TestGame)
        {
            // test game finished
            Debug.Log("Going to study Tutorial");
            SetState(ProtocolStates.StudyTutorial);
        }
        else if (currentState == ProtocolStates.StudyGame)
        {
            Debug.Log("Going to questionnaire");
            SetState(ProtocolStates.Questionnaire);
        }
    }

    



}