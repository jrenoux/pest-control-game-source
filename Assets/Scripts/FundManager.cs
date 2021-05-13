using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FundManager : MonoBehaviour
{
    GameController gameController;
    CoachManager coachManager;

    public Text fundValueText; // field used to dislay the current amount of money available for the active player
    public Text amountInput;

    private const int initFunds = 5; // the initial amount of money for each player
    private int[] availableFund; // the amount of money of each player. 
    private const int revenuePerYear = 2; // they money each player gets each turn from their paddies
    private int latestContribution = 0;


    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        coachManager = GameObject.Find("GameManager").GetComponent<CoachManager>();
       
        // init the funds for each player
        availableFund = new int[gameController.GetNbPlayers()];
        for (int i = 0; i < gameController.GetNbPlayers(); i++)
        {
            availableFund[i] = initFunds;
        };

        // display init fund
        fundValueText.text = availableFund[gameController.GetActivePlayerId()].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        fundValueText.text = availableFund[gameController.GetActivePlayerId()].ToString();
    }

    public void IncreaseAmount() 
    {

        int contribution = int.Parse(amountInput.text);
        if(contribution < availableFund[gameController.GetActivePlayerId()])
        {
            contribution = contribution + 1;
            amountInput.text = contribution.ToString();
        }
    }


    public void DecreaseAmount() 
    {

        int contribution = int.Parse(amountInput.text);
        if(contribution > 0)
        {
            contribution = contribution - 1;
            amountInput.text = contribution.ToString();
        }
    }

      // functions used by the buttons on the UI
    public void Pay() 
    {
        //called when the player clicks the "Pay" button. Starts the round manager
        latestContribution = int.Parse(amountInput.text);

        // the contribution is valid, we start the round
        // we remove the amount paid from available funds
        availableFund[gameController.GetActivePlayerId()] = availableFund[gameController.GetActivePlayerId()] - latestContribution;
        coachManager.InformAmountContributed(latestContribution);

        // and we tell the game manager that we are switching to the next state
        gameController.NextState();

        // and we empty the input field
        amountInput.text = "0";
    }


    public void CollectRevenue()
    {
        for(int i = gameController.GetPestLocation() + 1 ; i < gameController.GetNbPlayers() ; i++) 
        {
            availableFund[i] = availableFund[i] + revenuePerYear;
        }
        coachManager.InformRevenueEarned(revenuePerYear);
    }

    public int GetLatestContribution()
    {
        return latestContribution;
    }

}
