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
    private const int nbPlayers = 5; // the total number of players
    private List<Player> activePlayerList;
    
    private const int maxYear = 15; // the max number of years played. 
    private const double easeOfPestControl = 0.01; // how easy it is to stop the pest spreading

    private int? seed = null;

    private const Player.PlayerType artificialPlayerType = Player.PlayerType.EGOISTIC;

    private (int x, int y)  initialPestTile = (-2, -5);

    //////////////////////////////////////////////////////////////////////// private variables
    private int year = 1;
    private int humanPlayerId = 4; // id of the active player 

    private (int x, int y)[] farmsLocations = {
                                        (0, -5),
                                        (0, -3),
                                        (3, -4), 
                                        (-2, -1),
                                        (1, -1)
    };

    private GameStates currentGameState = GameStates.WaitingForPlayerInput;

    private PestController pestController;

    private Tile pestTile; // the tile used for the pest
    private Nullable<(int x, int y)> pestTileToAdd =  null;
    private System.Random random;
    private bool latestPestControlSuccess = false;

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



    //////////////////////////////////////////////////////////////////////// Other managers
    FundManager fundManager;

    // Start is called before the first frame update
    void Start()
    {
        string worldJson = File.ReadAllText(@"Config/world.json");
        theWorld = JsonConvert.DeserializeObject<World>(worldJson);
       
        InitWorld();

        if(seed == null)
        {
            random = new System.Random();
        }
        else
        {
            random = new System.Random(seed.Value);
        }
        
        
        pestController = new PestController(random, initialPestTile);

        // init the other UI managers
        fundManager = fundSection.GetComponent<FundManager>();
        
        // Init the player list
        // TODO

        activePlayerList = new List<Player>();
        for (int i = 0 ; i < nbPlayers ; i++) 
        {
            if (i != humanPlayerId)
            {
                activePlayerList.Add(new Player(i, artificialPlayerType, farmsLocations[i], fundManager, pestController, random));
            }
            else
            {
                activePlayerList.Add(new Player(i, Player.PlayerType.HUMAN, farmsLocations[i], fundManager, pestController, random));
                Tile farmTile = TilesResourcesLoader.GetOwnFarmTile();
                int farmX = farmsLocations[i].x;
                int farmY = farmsLocations[i].y;
                tilemap.SetTile(new Vector3Int(farmX, farmY, 0), farmTile);
            }
        }


        // init the variables we will use
        currentGameState = GameStates.Init;

        gameStateHasChanged = false;


        // init the game display
        yearValue.text = year.ToString();

        pestTile = TilesResourcesLoader.GetPestTile();
        tilemap.SetTile(new Vector3Int(initialPestTile.x, initialPestTile.y,0), pestTile);

        popupDialog.SetActive(false);
        confirmPopup.SetActive(true);
        confirmPopupText.text = "";
        initOverlay.SetActive(true);

        
    }

    private void InitWorld()
    {
        // 1. paint the tiles for the map


        // 2. paint the farms with the right colors


        // 3. check if pestProgression is empty of not (if empty: random, if not: scripted)


        // 4. pain the initial pest location

    }

    // Update is called once per frame
    void Update()
    {
        // update pest progression
        UpdatePestProgression();

        // play the state machine
        if(currentGameState == GameStates.GameEnded) 
        {
            // NOTHING to do
        }
        else
        {   
            // display the new stuff
            // update the year
            yearValue.text =  year.ToString();
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

    void UpdatePestProgression()
    {
        if(pestTileToAdd != null) 
        {
            // get the location of the new pest tile depending on the year
            Debug.Log("x = " + pestTileToAdd.Value.x + ", y = " + pestTileToAdd.Value.y);
            tilemap.SetTile(new Vector3Int(pestTileToAdd.Value.x, pestTileToAdd.Value.y, 0), pestTile);

            pestTileToAdd = null;
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
        foreach(Player player in activePlayerList)
        {
            player.CalculateContribution();
        }
        
        NextState();
    } 

    IEnumerator PerformPestControl()
    {
        SendNotification("Performing Pest Control");

        yield return new WaitForSeconds(3);

        int totalContribution = 0;

        foreach(Player player in activePlayerList)
        {
            totalContribution = totalContribution + player.GetContribution();
            Debug.Log("Player " + player.GetId() + " paid " + player.GetContribution());
        }

        double threshold = (easeOfPestControl * totalContribution) / (1 + easeOfPestControl * totalContribution);
        double p = random.NextDouble();
        Debug.Log("threshold = " + threshold);
        Debug.Log("p = " + p);
        
        if(p < threshold)
        {
            latestPestControlSuccess = true;
            SendNotification("The pest control was successful");
            
        }
        else
        {  
            latestPestControlSuccess = false;

            SendNotification("The pest control was unseccussful, the pest has progressed");
            pestTileToAdd = pestController.GetNextPestTile();

            // we check if the pest reached an artificial player
            var activePlayerCopy = new List<Player>(activePlayerList);
            foreach(Player pl in activePlayerCopy)
            {
                if(pl.GetFarmLocation().x == pestTileToAdd.Value.y 
                && pl.GetFarmLocation().y == pestTileToAdd.Value.y)
                {
                    // it has reached the player, which is out of the game
                    // we remove it from the list
                    activePlayerList.Remove(pl);
                    if(pl.IsHuman())
                    {
                        LoseGame();
                    }
                }
            }
        }

        if(currentGameState != GameStates.GameEnded)
        {
            NextState();
        }
    }

    void ConfirmPestControl() 
    {
        if(latestPestControlSuccess)
        {
            // TODO pest control success
            Debug.Log("Pest control success");
        }
        else
        {
            // TODO pest control failure
            Debug.Log("Pest control failure");
        }
        NextState();
    }

    void CollectRevenue()
    {
        foreach(Player player in activePlayerList)
        {
            player.CollectRevenue(fundManager.getRevenuePerYear());
        }
        SendNotification("You've earned " + fundManager.getRevenuePerYear() + " GP from your farm.");
        NextState();
    }

    void PrepareForNextYear()
    {
        year = year + 1;
        if(year > maxYear)
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
        Debug.Log("End of the game");

        // check
        ActivatePopup("GAME OVER \\ The Pest has reached your farm");
    }

    public void WinGame()
    {
        currentGameState = GameStates.GameEnded;
        // TODO
        // do stuff here
        Debug.Log("End of the game");

        ActivatePopup("CONGRATULATIONS \\ You have reached the end of the game");
  
    }

    public int GetNbPlayers() 
    {
        return nbPlayers;
    }

    public Player GetHumanPlayer()
    {
        return activePlayerList[humanPlayerId];
    }

}
