using UnityEngine;
using UnityEngine.UI;
public enum TutorialState
{
    Introduction,
    Context,
    Year,
    Farm,
    PlayerIcon,
    Wallet,
    Pest, 
    Collective,
    Contribution,
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
    private GameObject playerIcon;
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
                case TutorialState.Introduction:
                TutorialIntroduction();
                break;

                case TutorialState.Context:
                TutorialContext();
                break;

                case TutorialState.Farm:
                TutorialFarm();
                break;

                case TutorialState.PlayerIcon:
                TutorialPlayerIcon();
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

            if (currentState == 0)
            {
                tutorialController.DeactivateTutorialIntroduction();
            }

            currentState = (TutorialState)tutorialStepNumber; // we do it before increment becase the step actually
                                                              // starts at 1
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


    private void TutorialIntroduction()
    {
        tutorialController.DisplayIntroduction();
        Debug.Log("Tutorial Introduction");
    }


    private void TutorialContext()
    {
        string title = "<color=#52433D>Tutorial 1/10</color>";
        string message = "In this game each player controls a farm corresponding to one paddy in the board. " +
        "Each year your farm will give you profit and your goal is to maximize that profit by the end of the game."+
        "Yet, pest is spreading from abandoned paddies and endangering all the farms. \n \n" +
        "<b>If you lose your farm, you lose all your profit. </b>\n \n" + 
        "Let's go through the main components of the game. ";

        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText);

        Debug.Log("Tutorial Context");
    }

    private void TutorialYear()
    {
        // show the year and adapt the text to the game configuration
        Debug.Log("Tutorial Year");
        string title = "<color=#52433D>Tutorial 2/10</color>";
        string message = "This is the year counter. \n A game has a total duration of " + PestApplication.Instance.theWorld.maxYear + " years. \n \n "+
            "The game can end sooner for you if you do not survive a pest outbreak.";
        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, year);
    }

    private void TutorialFarm()
    {
        // show the tutorial window connected to the player farmer tile 
        Location farmLocation = PestApplication.Instance.theWorld.humanPlayer.farmLocation;

        string title = "<color=#52433D>Tutorial 3/10</color>";
        string message = "<b>This is your farm</b>. It is circled in black for you to see it better. \n \n" +
        "As long as the pest doesn't get you, your farm gives you <b>two coins per year.</b> ";

        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, farmLocation);

        Debug.Log("Tutorial Farm");
    }


    private void TutorialPlayerIcon()
    {

        string title = "<color=#52433D>Tutorial 4/10</color>";
        string message = "This is your player color that matches the color of your farm.";

        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, playerIcon);

        Debug.Log("Tutorial Farm");
    }

    private void TutorialWallet()
    {
        Debug.Log("Tutorial Wallet");
        // show the wallet
        string title = "<color=#52433D>Tutorial 5/10</color>";
        string message = "This is your wallet. It shows how many coins you've collected throughout the game. \n \n " +
            "If your're lucky and the pest doesn't get to your farm, at the end of the game, the money <b>in your wallet</b> will be converted into a <b>bonus</b>.";
        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, wallet);
    }

    private void TutorialPest()
    {
        // show the tutorial window connected to the pest farm
        Debug.Log("Tutorial Pest");
        Location pestLocation = PestApplication.Instance.theWorld.pestProgression.initialPestLocation;

        string title = "<color=#52433D>Tutorial 6/10</color>";
        string message = "A paddy of this color identifies the pest. \n \n" +
            "A pest outbreak occurs at an abandoned paddy and <b>spreads with a certain probability.</b> " +
        "If not controlled, it will directly threaten the frontier of any farmer adjacent to it. ";

        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, pestLocation);
    }

    private void TutorialCollective()
    {
        // show the tutorial window on the pest tile again
        Debug.Log("Tutorial Collective");
        Location pestLocation = PestApplication.Instance.theWorld.pestProgression.initialPestLocation;

        string title = "<color=#52433D>Tutorial 7/10</color>";
        string message = "To reduce the risk of pest spreading, farmers can contribute to an agricultural collective. " + 
        "The more coins collected, the less likely is the pest spreading. " +
        "Each year farmers must decide <b>if and how much they want to contribute</b> to the collective.";

        string buttonText = "Next";

        tutorialController.DisplayTutorialPanel(title, message, buttonText, pestLocation);
    } 
    

    private void TutorialContribution()
    {
        // show the up, down, and pay buttons
        Debug.Log("Tutorial Contribution");
        string title = "<color=#52433D>Tutorial 8/10</color>";
        string message = "A game turn starts setting a contribution to the agricultural collective. \n \n By moving arrows up and down and then hitting 'Pay', you decide how many coins you want to contribute to the collective, each year. " +
        "The coins you spend will be subtracted from your wallet. ";

        string buttonText = "Next";
        tutorialController.DisplayTutorialPanel(title, message, buttonText, contribution);
    }


    

    private void TutorialChat()
    {
        // show the chat window and adapt the text to the condition
        Debug.Log("Tutorial Chat");
        string title = "<color=#52433D>Tutorial 9/10</color>";
        string buttonText = "Next";
        string message = "";
        if(!PestApplication.Instance.chatManager.feedback.condition.Equals("control"))
        {
            message = "This box will contain information about the game progression." +
                "Throughout the game an artificial agent will give you information and feedback, so keep an eye on it!";
        }
        else
        {
            message = "This box will contain information about the game progression. Keep an eye on it!";    
        }


        tutorialController.DisplayTutorialPanel(title, message, buttonText, chat);
    }

    private void TutorialTestGame()
    {
        // window without arrow explaning the test game
        Debug.Log("Tutorial Test Game");
        string title = "<color=#52433D>Tutorial 10/10</color>";
        string buttonText = "Start Test Game";
        string message = "";

        if(!PestApplication.Instance.chatManager.feedback.condition.Equals("control"))
        {
            message = "Now you will play a <b><i>test game</i></b> against artificial players, so that you can familiarize yourself with the game. " + 
            "During this test game, the artificial players will always contribute the same amount of coins. \n\n" +
            "After you will be paired with human players to play the <b><i>real game</i></b>. \nYou will not receive feedback during the test game, only during the real game.  \n \n" +
            "The coins gathered during the <b><i>test game</i></b> will not be part of your final bonus.";
        }
        else
        {
            message = "Now you will play a <b><i>test game</i></b> against artificial players, so that you can familiarize yourself with the game. " + 
            "During this test game, the artificial players will always contribute the same amount of coins. \n\n" +
            "After you will be paired with human players to play the <b><i>real game</i></b>. \n \n" +
            "The coins gathered during the <b><i>test game</i></b> will not be part of your final bonus.";
        }
        

        

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
        string title = "Let's play!";
        string message = "You will now be connected to " + (PestApplication.Instance.theWorld.activePlayers.Count - 1) + " other players to play the study game. " + 
        "Remember to check the color of your farm at the top hand-left side of the window. Good luck!";
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