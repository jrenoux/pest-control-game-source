using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FundManager : MonoBehaviour
{
    GameControllerScript gameController;
    public Text fundValueText; // field used to dislay the current amount of money available for the active player
    public Text amountInput;

    private int latestContribution = 0;


    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameController.GameEnded())
        {
            if(gameController.GetHumanPlayer() != null)
            {
                fundValueText.text = gameController.GetHumanPlayer().wallet.ToString();
            }
        }
    }

    public void IncreaseAmount() 
    {

        int contribution = int.Parse(amountInput.text);
        if(contribution < gameController.GetHumanPlayer().wallet)
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
}
