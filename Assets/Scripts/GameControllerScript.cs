using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GameControllerScript : MonoBehaviour
{

    private enum GameStates {
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
    private const double easeOfPestControl = 0.2; // how easy it is to stop the pest spreading

    private const int seed = 314;

    private const Player.PlayerType artificialPlayerType = Player.PlayerType.PROSOCIAL;

    //////////////////////////////////////////////////////////////////////// private variables
    private int year = 1;
    private int humanPlayerId = 4; // id of the active player 
     // shows each turn which new tile becomes affected by the pest (x, y coordinates)
    private (int x, int y)[] pestProgression = {   
                                        (-2, -5), // initial pest location
                                        (-1, -4), // 1
                                        (-1, -3), // 2 
                                        (-1, -5), //3
                                        (0, -2), 
                                        (0, -5), //5
                                        (-1, -1), //6
                                        (0, -3), //7
                                        (-2, -1), 
                                        (0, -1), //9
                                        (1, -5), 
                                        (1, -3), 
                                        (1, -1)
                                    };

    private (int x, int y)[] farmsLocations = {
                                        (0, -5),
                                        (0, -3),
                                        (3, -4), 
                                        (-2, -1),
                                        (1, -1)
    };

    private int pestProgressionIndex = 0; // contain the  index of the current pest progression
    private GameStates currentGameState = GameStates.WaitingForPlayerInput;

    private Tile pestTile; // the tile used for the pest
    private bool pestTileToAdd = false;
    private System.Random random;
    private bool latestPestControlSuccess = false;

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
    

    //////////////////////////////////////////////////////////////////////// Other managers
    FundManager fundManager;

    // Start is called before the first frame update
    void Start()
    {
        // Init the player list
        // TODO

        activePlayerList = new List<Player>();
        for (int i = 0 ; i < nbPlayers ; i++) 
        {
            if (i != humanPlayerId)
            {
                activePlayerList.Add(new Player(i, artificialPlayerType, farmsLocations[i]));
            }
            else
            {
                activePlayerList.Add(new Player(i, Player.PlayerType.HUMAN, farmsLocations[i], fundManager));
            }
        }

        // init the other UI managers
        fundManager = fundSection.GetComponent<FundManager>();

        // init the variables we will use
        currentGameState = GameStates.WaitingForPlayerInput;
        random = new System.Random(seed);

        gameStateHasChanged = false;


        // init the game display
        yearValue.text = year.ToString();

        pestTile = TilesResourcesLoader.GetPestTile();
        int initPestX = pestProgression[0].x;
        int initPestY = pestProgression[0].y;
        tilemap.SetTile(new Vector3Int(initPestX, initPestY,0), pestTile);

        popupDialog.SetActive(false);
        confirmPopup.SetActive(false);

        
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
        if(pestTileToAdd) 
        {
            // get the location of the new pest tile depending on the year
            int pestTileX = pestProgression[pestProgressionIndex].x;
            int pestTileY = pestProgression[pestProgressionIndex].y;
            Debug.Log("x = " + pestTileX + ", y = " + pestTileY);
            tilemap.SetTile(new Vector3Int(pestTileX, pestTileY, 0), pestTile);

            pestTileToAdd = false;
        }
    }

    ////////////////////////////////////////////////// State Machine Functions
    public void NextState()
    {
        int stateId = (int)currentGameState;
        stateId = (stateId + 1 ) % (Enum.GetNames(typeof(GameStates)).Length -1 );
        currentGameState = (GameStates)stateId;
        gameStateHasChanged = true;
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
        ActivatePopup("Waiting for other players");
        int timeToWait = random.Next(2, 5);
        yield return new WaitForSeconds(timeToWait);
        DeactivatePopup();
     
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
        ActivatePopup("Performing Pest Control");

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


        DeactivatePopup();

        if(p < threshold)
        {
            latestPestControlSuccess = true;
            ActivateConfirmPopup("The pest control was successful");
            
        }
        else
        {  
            
            pestProgressionIndex = pestProgressionIndex + 1; 
            latestPestControlSuccess = false;
                        
            Debug.Log("Pest progression index = " + pestProgressionIndex);
            Debug.Log("pest progression length = " + pestProgression.Length);

            ActivateConfirmPopup("The pest control was unseccussful, the pest has progressed");

            // we check if the pest reached an artificial player
            foreach(Player pl in activePlayerList)
            {
                var pestX = pestProgression[pestProgressionIndex].x;
                var pestY = pestProgression[pestProgressionIndex].y;
                if(pl.GetFarmLocation().x == pestX && pl.GetFarmLocation().y == pestY)
                {
                    // it has reached the player, which is out of the game
                    // we remove it from the list
                    activePlayerList.Remove(pl);
                }
            }

            if(pestProgressionIndex == pestProgression.Length - 1)
            {
                EndGame();
            }
            

        }

        if(currentGameState != GameStates.GameEnded)
        {
            pestTileToAdd = true;
            
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
        ActivateConfirmPopup("You've earned " + fundManager.getRevenuePerYear() + " GP from your farm.");
    }

    void PrepareForNextYear()
    {
        year = year + 1;
        if(year > maxYear)
        {
            EndGame();
        }
        if(currentGameState != GameStates.GameEnded)
        {
            NextState();
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
        popupDialog.SetActive(false);    
    }

    private void ActivateConfirmPopup(string message) 
    {
        confirmPopupText.text = message;
        confirmPopup.SetActive(true);
    }

    private void DeactivateConfirmPopup()
    {
        confirmPopup.SetActive(false);
    }

    public void EndGame()
    {
        currentGameState = GameStates.GameEnded;
        // TODO
        // do stuff here
        Debug.Log("End of the game");
        
        if(pestProgressionIndex == pestProgression.Length - 1)
        {
            ActivatePopup("GAME OVER \\ The Pest has reached your farm");
        }
        else
        {
            ActivatePopup("CONGRATULATIONS \\ You have reached the end of the game");
        }
        
        
    }

    public int GetNbPlayers() 
    {
        return nbPlayers;
    }

    public Player GetHumanPlayer()
    {
        return activePlayerList[humanPlayerId];
    }

    public void OnConfirmPopupClicked() 
    {
        DeactivateConfirmPopup();
        NextState();
    }




}
