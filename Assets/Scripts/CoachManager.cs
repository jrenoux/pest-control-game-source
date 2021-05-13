using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class takes care of the artifical coach and updates the corresponding sections based on the information level that we want to use
public class CoachManager : MonoBehaviour
{
    enum InfoCollective { None, Total, Full };
    enum InfoPest {None, Neighbors, Full};
    private const int neighborLimit = 2;


    private const InfoCollective coachIcLevel = InfoCollective.Full;
    private const InfoPest coachIpLevel = InfoPest.Neighbors;


    // fields to link with the UI
    public Text messageText;

    private GameController gameController;
    private GridManager gridManager;

    private bool isReady = false;
    private int nbPlayers = 0;
    private int activePlayerId = -1;

    
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        StartCoroutine(InitCoachManager());
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(UpdateView());
    }

    private IEnumerator UpdateView()
    {
        yield return new WaitUntil(() => isReady);
        //update total contribution
         int totalContribution = 0;
        for(int i = 0; i < nbPlayers ; i++)
        {
            int contribution = gameController.GetContributionFrom(i);
            totalContribution = totalContribution + contribution;
            if(coachIcLevel == InfoCollective.Full) 
            {
                gridManager.UpdateContributionDisplay(i, contribution);
                
            }
            
        }
        if(coachIcLevel == InfoCollective.Full || coachIcLevel == InfoCollective.Total)
        {
            gridManager.UpdateTotalDisplay(totalContribution);
        }  
        
    }

    public void InformPestControlFailure(int newPestLocation)
    {
        string message = "";
        if(coachIpLevel == InfoPest.Full) 
        {
            message = "The pest has reached the farm of Player " + newPestLocation;
        }
        else if(coachIpLevel == InfoPest.Neighbors)
        {
            // checks how far away the pest is
            int distance = activePlayerId - newPestLocation;
            // if it was unsuccessful and is now close to the player
            if(distance <= neighborLimit)
            {
                message = "The pest has reached the farm of Player " + newPestLocation;
            }
        }
        messageText.text = message;
    }

    public void InformPestControlSuccess(int pestLocation)
    {
        string message = "";
        
        // check the level of the coach manager before sending the message
        if(coachIpLevel == InfoPest.Full) 
        {
            message = "The Pest Control was successful";
        }
        else if(coachIpLevel == InfoPest.Neighbors)
        {
            // checks how far away the pest is
            int distance = activePlayerId - pestLocation;
            // if it was unsuccessful and is now close to the player
            if(distance <= neighborLimit)
            {
                message = "The Pest Control was successful";
            }
        }

        messageText.text = message;
        // TODO there will be some graphic changes as well here
        
    }

    public void InformAmountContributed(int activePlayerContribution)
    {
        string message = "You have spent " + activePlayerContribution + " GP";
        messageText.text = message;
    }

    public void InformRevenueEarned(int revenue)
    {
        string message = "You have earned " + revenue + " GP from your part";
        messageText.text = message;
    }

    public IEnumerator InitCoachManager()
    {
        yield return new WaitUntil(() => gameController.IsReady());
        nbPlayers = gameController.GetNbPlayers();
        activePlayerId = gameController.GetActivePlayerId();

        if(coachIcLevel == InfoCollective.None)
        {
            GameObject.Find("CollectiveSection").SetActive(false);
        }
        else
        {
            gridManager = GameObject.Find("CollectiveGrid").GetComponent<GridManager>();
            gridManager.InitGrid(nbPlayers, (coachIcLevel == InfoCollective.Full));  
        }     
        messageText.text = "";
           
        isReady = true;
    }

}
