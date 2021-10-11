using UnityEngine;
using System;
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

    private string sessionId;

    public void Update()
    {
        if(hasStateChanged)
        {
            Debug.Log("Game State has changed. New state is " + currentState);
            switch(currentState)
            {
                case ProtocolStates.Start:
                    // nothing to do
                    break;

                case ProtocolStates.TestTutorial:
                    hasStateChanged = false;
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
        PestApplication.Instance.startGameController.DisplayInitOverlay();
    }

    ////////////////////////////////////////////////////State Machine Functions
    public void StartTestTutorial() 
    {
        // setting up the game
        PestApplication.Instance.SetupTestGame();

        long startTutorialTestTimestamp = PestApplication.Instance.GetCurrentTimestamp();
        Debug.Log(startTutorialTestTimestamp);
        PestApplication.Instance.gameConfig.SetStartTutorialTimestamp(startTutorialTestTimestamp);

        // starting the tutorial
        PestApplication.Instance.tutorialManager.StartTestTutorial();

    }
     
    public void StartTestGame()
    {
        Debug.Log("Start Test Game");
        // log the timestamp
        
        long startTestGameTimestamp = PestApplication.Instance.GetCurrentTimestamp();
        Debug.Log(startTestGameTimestamp);
        PestApplication.Instance.gameConfig.SetStartGameTimestamp(startTestGameTimestamp);
        PestApplication.Instance.logManager.SaveGameConfig(PestApplication.Instance.gameConfig);


        PestApplication.Instance.gameManager.StartGame(true);
        PestApplication.Instance.menuController.ActivateTestGameWatermark();
    }

    public void StartStudyTutorial() 
    {
        Debug.Log("StartStudyTutorial");
        PestApplication.Instance.SetupStudyGame();

        long startStudyTutorialTimestamp = PestApplication.Instance.GetCurrentTimestamp();
        Debug.Log(startStudyTutorialTimestamp);
        PestApplication.Instance.gameConfig.SetStartTutorialTimestamp(startStudyTutorialTimestamp);

        PestApplication.Instance.tutorialManager.StartStudyTutorial();
        PestApplication.Instance.menuController.DeactivateTestGameWatermark();


    }
    public void StartStudyGame()
    {
        Debug.Log("Start Study Game");

        long startStudyGameTimestamp = PestApplication.Instance.GetCurrentTimestamp();
        Debug.Log(startStudyGameTimestamp);
        PestApplication.Instance.gameConfig.SetStartGameTimestamp(startStudyGameTimestamp);

        // the config is finished, we upload the log
        PestApplication.Instance.logManager.SaveGameConfig(PestApplication.Instance.gameConfig);

        PestApplication.Instance.gameManager.StartGame(false);
    }

    public void StartQuestionnaire() 
    {
        Debug.Log("Start Questionnaires");
        PestApplication.Instance.menuController.ActivateCodePopup();
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
        PestApplication.Instance.prolificID = prolificID;
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
        Debug.Log("Game Finished");
        
        if(currentState == ProtocolStates.TestGame)
        {
            // test game finished
            long endTestGameTimestamp = PestApplication.Instance.GetCurrentTimestamp();
            DataEntryEndGame endGame = PestApplication.Instance.EndGame("test", endTestGameTimestamp);
            PestApplication.Instance.logManager.SaveEndGame(endGame);

            Debug.Log("Going to Study Tutorial");
            SetState(ProtocolStates.StudyTutorial);
        }
        else if (currentState == ProtocolStates.StudyGame)
        {
            long endStudyGameTimestamp = PestApplication.Instance.GetCurrentTimestamp();
            DataEntryEndGame endGame = PestApplication.Instance.EndGame("study", endStudyGameTimestamp);
            PestApplication.Instance.logManager.SaveEndGame(endGame);

            Debug.Log("Going to Questionnaire");
            SetState(ProtocolStates.Questionnaire);
        }
    }

    



}