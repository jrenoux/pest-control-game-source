using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GameControllerScript : MonoBehaviour
{

    private enum GameStates {
                                Init,
                                Tutorial,
                                WaitingForPlayerInput,
                                ConfirmPlayerInput,
                                WaitingForOtherPlayers,
                                PerformingPestControl,
                                ConfirmPestControl,
                                CollectRevenue,
                                PrepareForNextYear,
                                GameEnded
                            };


    private bool gameStateHasChanged = false;

    /////////////////////////////////////////////////////////////////////// game configuration

    private int? seed = null;

    //////////////////////////////////////////////////////////////////////// private variables
    private GameStates currentGameState = GameStates.WaitingForPlayerInput ;

    
    private System.Random random;
    private World theWorld;

    //////////////////////////////////////////////////////////////////////// UI attributes
    public Tilemap tilemap;
    public Text yearValue;
    public Button upButton;
    public Button downButton;
    public Button payButton;
    public GameObject fundSection;
    public GameObject popupDialog; // panel showed to wait for other players
    public Text popupDialogText;

    public GameObject confirmPopup;
    public Text confirmPopupText;

    public GameObject initOverlay;
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

    [SerializeField]
    private GameObject chatSection;




    //////////////////////////////////////////////////////////////////////// Other managers
    FundManager fundManager;
    ChatSectionControl chatManager;

    // Start is called before the first frame update
    void Start()
    {

        if(seed == null)
        {
            random = RandomSingleton.GetInstance();
        }
        else
        {
            random = RandomSingleton.GetInstance(seed.Value);
        }

        // init the world
        string worldJson = Resources.Load<TextAsset>(@"Config/world").text;
        this.theWorld = JsonConvert.DeserializeObject<World>(worldJson);
       
        theWorld.Init(this.tilemap);
        

        // init the other UI managers
        fundManager = fundSection.GetComponent<FundManager>();
        chatManager = chatSection.GetComponent<ChatSectionControl>();

        // init the variables we will use
        if(theWorld.tutorial)
        {
            currentGameState = GameStates.Init;
            initOverlay.SetActive(true);
            confirmPopup.SetActive(true);
        }
        else
        {
            currentGameState = GameStates.WaitingForPlayerInput;
            initOverlay.SetActive(false);
            tutorialPopup.SetActive(false); 
        }
        

        gameStateHasChanged = false;

        popupDialog.SetActive(false);
        confirmPopupText.text = "";



        // init the game display
        yearValue.text = theWorld.currentYear.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        // play the state machine
        if(currentGameState == GameStates.GameEnded) 
        {
            // NOTHING to do
        }
        else
        {   
            // display the new stuff
            // update the year
            yearValue.text =  theWorld.currentYear.ToString();
            // go to the next step if game state has changed
            if(gameStateHasChanged)
            {
                gameStateHasChanged = false;
                switch (currentGameState)
                {
                    case GameStates.Tutorial:
                        Debug.Log("State = Tutorial");
                        BlockPlayerInput();
                        UpdateTutorialPanel();
                        break;
                    case GameStates.WaitingForPlayerInput:
                        Debug.Log("State = Waiting for active player input");
                        PrepareForPlayerInput();
                        break;
                    case GameStates.ConfirmPlayerInput:
                        Debug.Log("State = Confirm player input");
                        ConfirmPlayerInput();
                        break;
                    case GameStates.WaitingForOtherPlayers:
                        Debug.Log("State = Waiting for other players input");
                        StartCoroutine(PlayArtificialPlayersRound());
                        break;
                    case GameStates.PerformingPestControl:
                        StartCoroutine(PerformPestControl());
                        Debug.Log("State = Performing pest control");
                        break;
                    case GameStates.ConfirmPestControl:
                        ConfirmPestControl();
                        Debug.Log("State = Confirm pest control status");
                        break;
                    case GameStates.CollectRevenue:
                        CollectRevenue();
                        Debug.Log("State = Collect revenue");
                        break;
                    case GameStates.PrepareForNextYear:
                        PrepareForNextYear();
                        Debug.Log("State = Prepare for next year");
                        break;
                    
                }

            }
        }
        
    }

    public void StartTutorial()
    {
        currentTutorial = 0;
        initOverlay.SetActive(false);
        NextState();
    }

    ////////////////////////////////////////////////// State Machine Functions
    public void NextState()
    {
        int stateId = (int)currentGameState;
        stateId = (stateId + 1 ) % (Enum.GetNames(typeof(GameStates)).Length -1 );
        currentGameState = (GameStates)stateId;
        gameStateHasChanged = true;
    }


    public void StartRound()
    {
        currentGameState = GameStates.WaitingForPlayerInput;
        gameStateHasChanged = true;
    }


    public void BlockPlayerInput()
    {
        // put the buttons and inactivable
        upButton.interactable = false;
        downButton.interactable = false;
        payButton.interactable = false;
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

    public void NextTutorial()
    {
        currentTutorial++;
        if (currentTutorial == tutorials.Length)
        {
            //end of tutorial andstart game
            tutorialPopup.SetActive(false);
            NextState();
        }
        else
        {
            UpdateTutorialPanel();
        }
    }


    public void PrepareForPlayerInput()
    {
        // put the buttons and all activable
        upButton.interactable = true;
        downButton.interactable = true;
        payButton.interactable = true;
    }

    void ConfirmPlayerInput()
    {
        // set the buttons as non interactable
        upButton.interactable = false;
        downButton.interactable = false;
        payButton.interactable = false;
        // TODO
        // send feedback from the coach
        if(chatManager.hasFeedback)
        {
            chatManager.SendFeedback(theWorld.currentYear, theWorld.pestProgression.latestPestSpread);
        }   
        else
        {

        }

        //called when the player clicks the "Pay" button. Starts the round manager
        int latestContribution = int.Parse(fundManager.amountInput.text);
        
        // the contribution is valid, we start the round
        // we remove the amount paid from available funds
        GetHumanPlayer().SetContribution(latestContribution);

        // and we empty the input field
        fundManager.amountInput.text = "0";

        NextState();
    }

    IEnumerator PlayArtificialPlayersRound()
    {
        SendNotification("Waiting for other players");
        int timeToWait = random.Next(2, 5);
        yield return new WaitForSeconds(timeToWait);
        //DeactivatePopup();
     
        // get other players' contribution
        // TODO
        foreach(Player player in theWorld.activePlayers)
        {
            player.CalculateContribution();
        }
        
        NextState();
    } 

    IEnumerator PerformPestControl()
    {
        int totalContribution = 0;

        foreach(Player player in theWorld.activePlayers)
        {
            totalContribution = totalContribution + player.GetContribution();
            Debug.Log("Player " + player.id + " paid " + player.GetContribution());
        }
        SendNotification("The collective gathered " + totalContribution + " coins.");
        //SendNotification("Performing Pest Control");

        yield return new WaitForSeconds(3);

        GridTile pestTile;
        switch(theWorld.pestProgression.type)
        {
            case "random":
            if(PestHasProgressed(totalContribution))
            {
                pestTile = theWorld.GetNextPestTileRandom();
                theWorld.SpawnPestTile(pestTile);
            }
            break;

            case "scripted": 
            pestTile = theWorld.GetNextPestTileScripted();
            theWorld.SpawnPestTile(pestTile);                    
            break;

            case "semiscripted":
            if(PestHasProgressed(totalContribution))
            {
                pestTile = theWorld.GetNextPestTileSemiScripted();
                theWorld.SpawnPestTile(pestTile);
            }
            // TODO
            break;

            default: 
            Debug.LogError("Pest progression type " + theWorld.pestProgression.type + " unknown. Known types are (random, scripted, semiscripted)");
            break;

        }
        if(!theWorld.activePlayers.Contains(theWorld.humanPlayer))
        {
            LoseGame();
        }
        if(currentGameState != GameStates.GameEnded)
        {
            NextState();
        }
    }

    private bool PestHasProgressed(int totalContribution)
    {
        double threshold = (theWorld.easeOfPestControl * totalContribution) / (1 + theWorld.easeOfPestControl * totalContribution);
        double p = random.NextDouble();
        Debug.Log("p: " + p + ", threshold: " + threshold);
        
        if(p < threshold)
        {
            SendNotification("The pest control was successful");
            return false;
        }
        else
        {  
            SendNotification("The pest control was unseccussful");
            return true;  
        }

       
    }

    void ConfirmPestControl() 
    {
        NextState();
    }

    void CollectRevenue()
    {
        foreach(Player player in theWorld.activePlayers)
        {
            player.CollectRevenue();
        }
        SendNotification("You've earned " + theWorld.humanPlayer.revenuePerYear + " GP from your farm.");
        NextState();
    }

    void PrepareForNextYear()
    {
        theWorld.currentYear = theWorld.currentYear + 1;
        if(theWorld.currentYear > theWorld.maxYear)
        {
            WinGame();
        }
        if(currentGameState != GameStates.GameEnded)
        {
            StartRound();
        }

    }


    ///////////////////////////////////////////////////////////// Other functions
    private void ActivatePopup(string message) 
    {
        popupDialogText.text = message;
        popupDialog.SetActive(true);
    }

    private void DeactivatePopup()
    {
        popupDialogText.text = "";
        popupDialog.SetActive(false);    
    }

    private void SendNotification(string message) 
    {
        confirmPopupText.text = message;
        confirmPopup.SetActive(true);
    }

    public void LoseGame()
    {
        currentGameState = GameStates.GameEnded;
        // TODO
        // do stuff here

        // check
        ActivatePopup("GAME OVER \\ The Pest has reached your farm");
    }

    public void WinGame()
    {
        currentGameState = GameStates.GameEnded;
        // TODO
        // do stuff here

        ActivatePopup("CONGRATULATIONS \\ You have reached the end of the game");
  
    }

    public Player GetHumanPlayer()
    {
        return theWorld.humanPlayer;
    }

    public bool GameEnded()
    {
        return currentGameState == GameStates.GameEnded;
    }
}
