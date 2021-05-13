using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{    
    // variables to configure
    private const int nbPlayers = 5; // the total number of players
    
    private const int maxYear = 10; // the max number of years played. 
    private const double easeOfPestControl = 1.0; //
    

    //  variables that are going to be updated during the game. 
    private int year = 1;
    private int activePlayer; // the id of the human player, is randomly chosen each time
    private int pestLocation = -1; // contains the location of the pest (only the highest number)
    private bool gameEnded = false; // put to true when the current player lost their paddy or when maxYear has been reached
    private int[] contributionPerPlayer;


    // attributes for the UI
    public Text yearText; // field used to display the year currently being played
    public Text playerText; // field used to display the active player number
    public GameObject popupDialog; // panel showed to wait for other players
    public Text popupDialogText;


    // references to the other controllers we'll need to use
    CoachManager coachManager;
    FundManager fundManager;

    // other useful variables
    private System.Random random;

    private bool isReady = false;


    // Start is called before the first frame update
    void Start() 
    {

        coachManager = GameObject.Find("GameManager").GetComponent<CoachManager>();
        fundManager = GameObject.Find("FundSection").GetComponent<FundManager>();
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

        contributionPerPlayer = new int[nbPlayers];
        for(int i = 0 ; i < nbPlayers ; i++)
        {
            contributionPerPlayer[i] = 0;
        }

        isReady = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(gameEnded)
        {
            // display end of the game message
            if(pestLocation == activePlayer)
            {
                // the player has lost due the pest spreading to their paddy
                // TODO
            }
            else
            {
                // the player has made it through the max number of rounds
                // TODO
            }

        }
        else
        {
            // display the new stuff
            // update the year
            yearText.text =  year.ToString();
        }
    }

  

    public void PlayRound(int playerContribution) 
    {
        StartCoroutine(PlayGameRound(playerContribution)); 
    }


    private int GetContribution(int agentNb) 
    {
        int contribution = 0;
        contributionPerPlayer[agentNb] = contribution;
        return contribution;
    }

    private IEnumerator PerformPestControl(int totalContribution)
    {
        ActivatePopup("Performing Pest Control");
        yield return new WaitForSeconds(3);
        double threshold = (easeOfPestControl * totalContribution) / (1 + easeOfPestControl * totalContribution);
        double p = random.NextDouble();
        Debug.Log("threshold = " + threshold);
        Debug.Log("p = " + p);
        DeactivatePopup();

        if(p < threshold)
        {
            coachManager.InformPestControlSuccess(pestLocation);
            // do some fancy animation stuff?
            // TODO
            StartCoroutine(fundManager.CollectRevenue());

            year = year + 1;
            if(year > maxYear)
            {
                gameEnded = true;
            }
        }
        else
        {  
            pestLocation = pestLocation + 1; 
            coachManager.InformPestControlFailure(pestLocation);
           
            // we check if the pest reached the current player
            if(pestLocation == activePlayer) 
            {
                // end of the game
                gameEnded = true;
            }
            else
            {
                // do some fancy animation stuff?
                // TODO
                StartCoroutine(fundManager.CollectRevenue());

                year = year + 1;
                if(year > maxYear)
                {
                    gameEnded = true;
                }
            }
            
        }


        

    }

    private int GetContributionFromOtherPlayers() 
    {
        int totalContribution = 0;
        for(int i = pestLocation + 1; i < nbPlayers ; i++) 
        {
            // we don't get the contribution of the active player
            if(i != activePlayer) 
            {
                int playerContribution = GetContribution(i);
                contributionPerPlayer[i] = playerContribution;
                totalContribution = totalContribution + GetContribution(i);
            }
        }
        return totalContribution;
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

    IEnumerator PlayGameRound(int activePlayerContribution)
    {
        ActivatePopup("Waiting for other players");
        int timeToWait = random.Next(2, 10);
        yield return new WaitForSeconds(timeToWait);
        DeactivatePopup();
     
        // update each agent's contribution
        contributionPerPlayer[activePlayer] = activePlayerContribution;

        // get other players' contribution
        int totalContribution = GetContributionFromOtherPlayers() + activePlayerContribution;
        

        StartCoroutine(PerformPestControl(totalContribution));

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

    public int GetContributionFrom(int playerId)
    {
        return contributionPerPlayer[playerId];
    }
}
