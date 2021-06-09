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
    
    private const int maxYear = 10; // the max number of years played. 
    private const double easeOfPestControl = 1.0; // how easy it is to stop the pest spreading

    //////////////////////////////////////////////////////////////////////// private variables
    private int year = 1;
    private int activePlayer = 4; // id of the active player 
     // shows each turn which new tile becomes affected by the pest (x, y coordinates)
    private int[] pestProgression = {   -2, -5, // initial pest location
                                        -2, -4,
                                        -1, -5,
                                    };
    private int pestProgressionIndex = 0; // contain the  index of the current pest progression
    private GameStates currentGameState = GameStates.WaitingForPlayerInput;

    private Tile pestTile; // the tile used for the pest
    private bool pestTileToAdd = false;
    private System.Random random;
    private int[] contributionPerPlayer; // contains the list of contribution for all of the players
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
    

    //////////////////////////////////////////////////////////////////////// Other managers
    FundManager fundManager;

    // Start is called before the first frame update
    void Start()
    {

        // init the other UI managers
        fundManager = fundSection.GetComponent<FundManager>();

        // init the variables we will use
        currentGameState = GameStates.WaitingForPlayerInput;
        random = new System.Random();

        gameStateHasChanged = false;
        contributionPerPlayer = new int[nbPlayers];
        for(int i = 0 ; i < nbPlayers ; i++)
        {
            contributionPerPlayer[i] = 0;
        }


        // init the game display
        yearValue.text = year.ToString();

        pestTile = TilesResourcesLoader.GetPestTile();
        int initPestX = pestProgression[0];
        int initPestY = pestProgression[1];
        tilemap.SetTile(new Vector3Int(initPestX, initPestY,0), pestTile);

        popupDialog.SetActive(false);

        
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
            int pestTileX = pestProgression[pestProgressionIndex * 2];
            int pestTileY = pestProgression[pestProgressionIndex * 2 + 1];
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
     
        // update active player's contribution
        contributionPerPlayer[activePlayer] = fundManager.GetLatestContribution();

        // get other players' contribution
        // TODO
        
        NextState();
    } 

    IEnumerator PerformPestControl()
    {
        ActivatePopup("Performing Pest Control");

        yield return new WaitForSeconds(3);

        int totalContribution = 0;
        for(int i = 0 ; i < nbPlayers ; i++)
        {
            totalContribution = totalContribution + contributionPerPlayer[i];
        }

        double threshold = (easeOfPestControl * totalContribution) / (1 + easeOfPestControl * totalContribution);
        double p = random.NextDouble();
        Debug.Log("threshold = " + threshold);
        Debug.Log("p = " + p);


        DeactivatePopup();

        if(p < threshold)
        {
            latestPestControlSuccess = true;
            
        }
        else
        {  
            pestProgressionIndex = pestProgressionIndex + 1; 
            latestPestControlSuccess = false;
           
            // we check if the pest reached the current player
            // TODO
            Debug.Log("Pest progression index = " + pestProgressionIndex);
            Debug.Log("pest progression length = " + pestProgression.Length);

            if(pestProgressionIndex == pestProgression.Length / 2)
            {
                EndGame();
            }

        }

        if(currentGameState != GameStates.GameEnded)
        {
            pestTileToAdd = true;
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
        fundManager.CollectRevenue();
        NextState();
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

    private int GetContribution(int playerId)
    {
        // TODO 
        // and to move in another file
        return 0;
    }

    public void EndGame()
    {
        currentGameState = GameStates.GameEnded;
        // TODO
        // do stuff here
        Debug.Log("End of the game");
        
        if(pestProgressionIndex == pestProgression.Length / 2)
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

    public int GetActivePlayerId()
    {
        return activePlayer;
    }




}
