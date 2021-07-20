using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FundManager : MonoBehaviour
{
    GameControllerScript gameController;
    public Text fundValueText; // field used to dislay the current amount of money available for the active player
    public Text amountInput;
    public Button upButton;
    public Button downButton;
    public Button payButton;

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

            if(gameController.IsWaitingForPlayerInput())
            {
                upButton.interactable = true;
                downButton.interactable = true;
                payButton.interactable = true;
            }
            else
            {
                upButton.interactable = false;
                downButton.interactable = false;
                payButton.interactable = false;
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

        gameController.NextState();
        
        
    }
}
