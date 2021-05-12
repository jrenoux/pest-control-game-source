using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FundManager : MonoBehaviour
{
    GameController gameController;
    EventMessageManager eventManager;

    public Text fundValueText; // field used to dislay the current amount of money available for the active player
    public Text amountInput;

    private const int initFunds = 5; // the initial amount of money for each player
    private int[] availableFund; // the amount of money of each player. 
    private const int revenuePerYear = 2; // they money each player gets each turn from their paddies


    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        eventManager = GameObject.Find("EventSection").GetComponent<EventMessageManager>();
       
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
        int contribution = int.Parse(amountInput.text);

        // the contribution is valid, we start the round
        // we remove the amount paid from available funds
        availableFund[gameController.GetActivePlayerId()] = availableFund[gameController.GetActivePlayerId()] - contribution;
        eventManager.CoachSays("You have spent " + contribution + " GP");
        // and we play the round (with coach's feedback and all
        gameController.PlayRound(contribution);

        // and we empty the input field
        amountInput.text = "0";
    }


    public IEnumerator CollectRevenue()
    {
        yield return new WaitForSeconds(5);
        for(int i = gameController.GetPestLocation() + 1 ; i < gameController.GetNbPlayers() ; i++) 
        {
            availableFund[i] = availableFund[i] + revenuePerYear;
        }

        eventManager.CoachSays("You've earned " + revenuePerYear + " GP from your farm");
    }

}
