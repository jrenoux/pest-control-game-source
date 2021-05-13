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
    private const InfoPest coachIpLevel = InfoPest.Full;


    // fields to link with the UI
    public Text messageText;

    private GameController gameController;
    private GridManager gridManager;

    private bool isReady = false;
    private int nbPlayers = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        gridManager = GameObject.Find("CollectiveGrid").GetComponent<GridManager>();
        if(coachIcLevel == InfoCollective.None)
        {
            GameObject.Find("CollectiveSection").SetActive(false);
        }
        else
        {
            StartCoroutine(InitCollectiveSection());
        }
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

    public void InformPestControlSuccess(bool successful)
    {
        string message = "";
        if(successful)
        {
            message = "The Pest Control was successful";
        }   
        else
        {
            message = "The Pest Cotnrol was unsuccessful";
        }

        // checks the level of the coach manager
        messageText.text = message;
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

    public IEnumerator InitCollectiveSection()
    {
        yield return new WaitUntil(() => gameController.IsReady());

        nbPlayers = gameController.GetNbPlayers();
        
        gridManager.InitGrid(nbPlayers, (coachIcLevel == InfoCollective.Full));      

        isReady = true;

    }

}
