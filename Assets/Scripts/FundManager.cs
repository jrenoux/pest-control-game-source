using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FundManager : MonoBehaviour
{
    GameControllerScript gameController;
    public Text fundValueText; // field used to dislay the current amount of money available for the active player
    public Text amountInput;

    private const int initFunds = 5; // the initial amount of money for each player
    private int[] availableFund; // the amount of money of each player. 
    private const int revenuePerYear = 2; // they money each player gets each turn from their paddies
    private int latestContribution = 0;


    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameControllerScript>();
       
        // init the funds for each player
        availableFund = new int[gameController.GetNbPlayers()];
        for (int i = 0; i < gameController.GetNbPlayers(); i++)
        {
            availableFund[i] = initFunds;
        };

        // TODO wait for manager to start
        // display init fund
        fundValueText.text = gameController.GetHumanPlayer().GetFund().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        fundValueText.text = gameController.GetHumanPlayer().GetFund().ToString();
    }

    public void IncreaseAmount() 
    {

        int contribution = int.Parse(amountInput.text);
        if(contribution < gameController.GetHumanPlayer().GetFund())
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
        gameController.GetHumanPlayer().SetContribution(latestContribution);


        // and we tell the game manager that we are switching to the next state
        gameController.NextState();

        // and we empty the input field
        amountInput.text = "0";
    }

    public int GetLatestContribution()
    {
        return latestContribution;
    }

    public int getRevenuePerYear() 
    {
        return revenuePerYear;
    }

}
