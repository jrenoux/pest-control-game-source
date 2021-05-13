using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    private enum GameStates {
                            WaitingForPlayerInput,
                            ConfirmPlayerInput, 
                            WaitingForOtherPlayers, 
                            PerformingPestControl, 
                            ConfirmPestControl, 
                            CollectRevenue,
                            PrepareForNextYear,
                            GameEnded} ;

    
     /////////////////////////////////////////////////////////////////////// variables to configure
    private const int nbPlayers = 5; // the total number of players
    
    private const int maxYear = 10; // the max number of years played. 
    private const double easeOfPestControl = 1.0; //

    //////////////////////////////////////////////////////////////////////// private variables
    private int year = 1;
    private int activePlayer; // the id of the human player, is randomly chosen each time
    private int pestLocation = -1; // contains the location of the pest (only the highest number)
    private int[] contributionPerPlayer;
    private GameStates currentGameState;
    private System.Random random;
    private bool isReady = false;
    private bool latestPestControlSuccess = false;
    private bool gameStateHasChanged = false;

    CoachManager coachManager;
    FundManager fundManager;


    ////////////////////////////////////////////////////////////////////////// attributes for the UI
    public Text yearText; // field used to display the year currently being played
    public Text playerText; // field used to display the active player number
    public GameObject popupDialog; // panel showed to wait for other players
    public Text popupDialogText;
    public GameObject fundSection;
    public Button upButton;
    public Button downButton;
    public Button payButton;
    public Text endOfGameText;
    public Text reasonGameOverText;
    public GameObject endGamePanel;


    // Start is called before the first frame update
    void Start()
    {
        currentGameState = GameStates.WaitingForPlayerInput;
        coachManager = GameObject.Find("GameManager").GetComponent<CoachManager>();
        fundManager = fundSection.GetComponent<FundManager>();
        // init the random generator
        random = new System.Random();
        // Choose the id of the active player
        // TODO to change to make it random at start
        activePlayer = random.Next(0, nbPlayers);

        playerText.text =  activePlayer.ToString();
    
        // display year
        yearText.text = year.ToString();

        // make sure that the fake waiting box is not displayed
        popupDialog.SetActive(false); 
        endGamePanel.SetActive(false);

        contributionPerPlayer = new int[nbPlayers];
        gameStateHasChanged = false;

        for(int i = 0 ; i < nbPlayers ; i++)
        {
            contributionPerPlayer[i] = 0;
        }

        isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentGameState == GameStates.GameEnded)
        {
            // nothing to do
        }
        else
        {
            // display the new stuff
            // update the year
            yearText.text =  year.ToString();
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

    public void NextState()
    {
        int stateId = (int)currentGameState;
        stateId = (stateId + 1 ) % (Enum.GetNames(typeof(GameStates)).Length -1 );
        currentGameState = (GameStates)stateId;
        gameStateHasChanged = true;
    }

    public void EndGame()
    {
        currentGameState = GameStates.GameEnded;
        // TODO
        // do stuff here
        if(pestLocation == activePlayer)
        {
            endOfGameText.text = "GAME OVER";
            reasonGameOverText.text = "The pest has reached your farm";
        }
        else
        {
            endOfGameText.text = "CONGRATULATIONS";
            reasonGameOverText.text = "You reached the end of the game";
        }
        endGamePanel.SetActive(true);
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
        coachManager.InformAmountContributed(fundManager.GetLatestContribution());
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
        for(int i = pestLocation + 1; i < nbPlayers ; i++) 
        {
            // we don't get the contribution of the active player
            if(i != activePlayer) 
            {
                int playerContribution = GetContribution(i);
                contributionPerPlayer[i] = playerContribution;
            }
        }
        
        NextState();
    } 

    private int GetContribution(int agentNb) 
    {
        int contribution = 0;
        return contribution;
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
            pestLocation = pestLocation + 1; 
            latestPestControlSuccess = false;
           
            // we check if the pest reached the current player
            if(pestLocation == activePlayer) 
            {
                // end of the game
                EndGame();
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
            coachManager.InformPestControlSuccess(pestLocation);
        }
        else
        {
            coachManager.InformPestControlFailure(pestLocation);
        }
    }

    void CollectRevenue()
    {
        fundManager.CollectRevenue();
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

    private void ActivatePopup(string message) 
    {
        popupDialogText.text = message;
        popupDialog.SetActive(true);
    }

    private void DeactivatePopup()
    {
        popupDialog.SetActive(false);    
    }

    public bool IsReady() 
    {
       return isReady;
    }
    public int GetNbPlayers() 
    {
        return nbPlayers;
    }

    public int GetActivePlayerId()
    {
        return activePlayer;
    }

    public int GetPestLocation()
    {
        return pestLocation;
    }
    public int GetContributionFrom(int playerId)
    {
        return contributionPerPlayer[playerId];
    }
}
