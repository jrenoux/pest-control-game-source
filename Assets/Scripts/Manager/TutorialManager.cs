using UnityEngine;
using UnityEngine.UI;
public enum TutorialState
{
    Context,
    Farm, 
    Pest, 
    Collective, 
    Wallet, 
    Contribution, 
    Year,
    Chat,
    TestGame, 
    WaitingEndTestGame,
    StudyGame, 
    End
}
public class TutorialManager : MonoBehaviour
{

    // list of the uiGameObjects the tutorial popup sticks to
    [SerializeField]
    private GameObject wallet;
    [SerializeField]
    private GameObject contribution;
    [SerializeField]
    private GameObject year;
    [SerializeField]
    private GameObject chat;


    private bool hasStateChanged = false;
    private TutorialState currentState = TutorialState.Farm;
    private int tutorialStepNumber = 1;

    private TutorialController tutorialController;

    public void Start()
    {
        tutorialController = PestApplication.Instance.tutorialController;
        tutorialController.DeactivateTutorialPopup();
        
    }

    public void Update()
    {
        if(hasStateChanged)
        {
            switch(currentState)
            {
                case TutorialState.Context:
                TutorialContext();
                break;

                case TutorialState.Farm:
                TutorialFarm();
                break;

                case TutorialState.Pest:
                TutorialPest();
                break;

                case TutorialState.Collective:
                TutorialCollective();
                break;

                case TutorialState.Wallet:
                TutorialWallet();
                break;

                case TutorialState.Contribution:
                TutorialContribution();
                break;

                case TutorialState.Year:
                TutorialYear();
                break;

                case TutorialState.Chat:
                TutorialChat();
                break;

                case TutorialState.TestGame:
                TutorialTestGame();
                break;

                case TutorialState.WaitingEndTestGame:
                TutorialWaitingEndTestGame();
                break;

                case TutorialState.StudyGame:
                TutorialStudyGame();
                break;

                case TutorialState.End:
                Debug.Log("Tutorial End");
                PestApplication.Instance.protocolManager.TutorialFinished();
                tutorialController.DeactivateTutorialPopup();
                break;
            }
            hasStateChanged = false;
        }
    }


    public void NextTutorial() 
    {
        if(currentState != TutorialState.End)
        {
            currentState = (TutorialState)tutorialStepNumber; // we do it before increment becase the step actually starts at 1.     
            tutorialStepNumber++;   
            hasStateChanged = true;
        }
        else
        {
            // should not really happen
            Debug.LogError("This should not be happening");
        }
        
    }

    public void StartTestTutorial()
    {
        currentState = (TutorialState)0;
        tutorialStepNumber = 1;
        hasStateChanged = true;
        // deactivate the menu
        PestApplication.Instance.menuController.DeactivateMenu();
    }

    public void StartStudyTutorial()
    {
        // deactivate the menu
        PestApplication.Instance.menuController.DeactivateMenu();
        NextTutorial();
    }

    private void TutorialContext()
    {
        string title = "Tutorial 1/9";
        string message = "In this game, each player controls one farm on the board." +
        "Your goal as a player is to reach the end of the game while collecting as much coins as possible." +
        "But beware: a pest is spreading through the land and endangering the farms." + 
        "Let's go through the main components of the game";

        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText);

        Debug.Log("Tutorial Context");
    }

    private void TutorialFarm()
    {
        // show the tutorial window connected to the player farmer tile 
        Location farmLocation = PestApplication.Instance.theWorld.humanPlayer.farmLocation;

        string title = "Tutorial 2/9";
        string message = "This is your farm. It is circled in black for you to see it better. " +
        "You can also see your player color in the top left circle. " +
        "Your farm gives you two coins per year as long as it is safe from the pest. ";

        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, farmLocation);

        Debug.Log("Tutorial Farm");
    }

    private void TutorialPest()
    {
        // show the tutorial window connected to the pest farm
        Debug.Log("Tutorial Pest");
        Location pestLocation = PestApplication.Instance.theWorld.pestProgression.initialPestLocation;

        string title = "Tutorial 3/9";
        string message = "This is the pest. " + 
        "If the farmers don't do anything, it will spread each year to one neighboring tile. ";

        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, pestLocation);
    }

    private void TutorialCollective()
    {
        // show the tutorial window on the pest tile again
        Debug.Log("Tutorial Collective");
        Location pestLocation = PestApplication.Instance.theWorld.pestProgression.initialPestLocation;

        string title = "Tutorial 4/9";
        string message = "To reduce the risk of the pest spreading, farmers can contribute coins to a collective. " + 
        "The more coins are collected, the less likely the pest will spread. ";

        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, pestLocation);
    } 
    private void TutorialWallet()
    {
         Debug.Log("Tutorial Wallet");
        // show the wallet
        string title = "Tutorial 5/9";
        string message = "This is your wallet. It shows how much coins you have each year.";
        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, wallet);
    }

    private void TutorialContribution()
    {
        // show the up, down, and pay buttons
        Debug.Log("Tutorial Contribution");
        string title = "Tutorial 6/9";
        string message = "This is where you decide each year how much coins you want to contribute to the collective. " +
        "The coins you spend will be removed from your wallet. ";

        string buttonText = "Next";
        tutorialController.DisplayTutorialPanel(title, message, buttonText, contribution);
    }


    private void TutorialYear()
    {
        // show the year and adapt the text to the game configuration
        Debug.Log("Tutorial Year");
        string title = "Tutorial 7/9";
        string message = "This is the year counter. To complete the game, you must reach year " + PestApplication.Instance.theWorld.maxYear + ". ";
        string buttonText = "Next";
        
        tutorialController.DisplayTutorialPanel(title, message, buttonText, year);
    }

    private void TutorialChat()
    {
        // show the chat window and adapt the text to the condition
        Debug.Log("Tutorial Chat");
        string title = "Tutorial 8/9";
        string message = "This box will contain information about the game progression. Keep an eye on it!";
        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, chat);
    }

    private void TutorialTestGame()
    {
        // window without arrow explaning the test game
        Debug.Log("Tutorial Test Game");
        string title = "Tutorial 9/9";
        string message = "Now you will play a test game against artificial players, so that you can familiarize yourself with the game. " + 
        "Later you will be paired with human players to play the real game. " + 
        "During this test game, the artificial players will always contributed the same amount of coins. "; 

        string buttonText = "Start Test Game";

        tutorialController.DisplayTutorialPanel(title, message, buttonText);
    }

    private void TutorialWaitingEndTestGame()
    {
        // deactivate the window
        PestApplication.Instance.tutorialController.DeactivateTutorialPopup();
        // warns the protocol manager that the first part of the tutorial is finished
        PestApplication.Instance.protocolManager.TutorialFinished();
        Debug.Log("Tutorial Waiting End Test Game");
    }

    private void TutorialStudyGame()
    {
        // window without arrow explaining the real game
        Debug.Log("Tutorial Study Game");
        string title = "Ready to play!";
        string message = "You will now we connected to " + (PestApplication.Instance.theWorld.activePlayers.Count - 1) + " othe players to play the study game. " + 
        "Rembemer to check which color is your farm on the top left. Good luck!";
        string buttonText = "Start Game";

        tutorialController.DisplayTutorialPanel(title, message, buttonText);
    }


    // public TutorialStep[] testTutorials = {
    //                                     new TutorialStep(new GridTile(GridTile.GridTileType.FARM, PestApplication.Instance.theWorld.humanPlayer.farmLocation), "Tutorial 1 / 7", "Next", "This is your farm, you can also see its color on the top menu. It gives you 2GP per year as long as it's safe from the pest. If the pest reaches you, you lose the game."), 
    //                                     new TutorialStep(640, 180, "Tutorial 2 / 7", "Next", 360, 170, 270, "This is the pest. If farmers don't do anything, it will spread each turn to one neighboring tile. To prevent the spreading, a collective of all the farmers have been formed."),
    //                                     new TutorialStep(400, 420, "Tutorial 3 / 7", "Next", 440, 160, 0, "Each farmer can contribute GP to the collective, and the more GP the collective gathers, the more efficient it will be to prevent the spread."),
    //                                     new TutorialStep(330, 420, "Tutorial 4 / 7", "Next", 300, 160, 0, "This is were you choose how much you want to contribute to the collective this year."),
    //                                     new TutorialStep(230, 420, "Tutorial 5 / 7", "Next", 100, 160, 0, "This is the amount of GP you have available."),
    //                                     new TutorialStep(240, 240, "Tutorial 6 / 7", "Next", 100, 480, 180, "This is the year counter. If you reach year 15, you win the game!"),
    //                                     new TutorialStep(240, 240, "Tutorial 7 / 8", "Next", 100, 480, 180, "In this box you will see information about the game as it plays, such as the amount of money collected by the collective, the result of the pest control... Keep an eye on it!"),
    //                                     new TutorialStep(302, 270, "Tutorial 7 / 7", "Start Test Game", "You will first play a test game with artificial players, always contributing the same amont. Feel free to experiment! Later, you will be connected with human players to play the real game.")
    // };

    // public TutorialStep[] studyTutorial = {
    //     new TutorialStep(240, 240, "Tutorial 1/2", "Next", "You will now be connected to four other players and play the study game.")
    // };


    
}