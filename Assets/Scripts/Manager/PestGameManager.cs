using UnityEngine;
using System.Collections;
using System;

public enum GameStates 
{
    WaitingToStart,
    WaitingForPlayerInput,
    ConfirmPlayerInput,
    ProcessPlayerInput,
    WaitingForOtherPlayers,
    PerformingPestControl,
    ConfirmPestControl,
    CollectRevenue,
    PrepareForNextYear,
    GameEnded
};

// this manager only manages the game itself. 
public class PestGameManager : MonoBehaviour
{
    private bool gameStateHasChanged = false;
    private int? seed = null; // TODO how do I deal with this? 
    private GameStates currentGameState = GameStates.WaitingForPlayerInput;

    private System.Random random; 
    private bool isTestGame;

    public void Awake()
    {
        random =  RandomSingleton.GetInstance(seed);
    }

    public void Update()
    {
        if(gameStateHasChanged)
        {
            gameStateHasChanged = false;
            switch (currentGameState)
            {
                case GameStates.WaitingToStart: 
                    Debug.Log("State = Waiting for protocol manager to start the game");
                    break;
                case GameStates.WaitingForPlayerInput:
                    Debug.Log("State = Waiting for active player input");
                    WaitingForPlayerInput();
                    break;
                case GameStates.ConfirmPlayerInput:
                    Debug.Log("State = Confirm player input");
                    ConfirmPlayerInput();
                    break;
                case GameStates.ProcessPlayerInput:
                    Debug.Log("State = Process player input");
                    ProcessPlayerInput();
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


    ///////////////////////////////////////////////////////////////////// State Machine Methods
    private void SetState(GameStates newState)
    {
        currentGameState = newState;
        gameStateHasChanged = true;

    }

    public void StartGame(bool isTestGame) 
    {
        Application.Instance.menuController.Reset();
        Application.Instance.chatManager.Reset();
        this.isTestGame = isTestGame;
        // fake other players' connection
        if(!this.isTestGame)
        {
            StartCoroutine(WaitForOtherPlayersToConnect());
        }
        else
        {
            SetState(GameStates.WaitingForPlayerInput);
        }
        
    }

    private void WaitingForPlayerInput()
    {
        Application.Instance.menuController.ActivateMenu();
    }

    private void ConfirmPlayerInput()
    {
        Debug.Log("PestGameManager.ConfirmPlayerInput");
        Application.Instance.menuController.DeactivateMenu();
        // we send feedback through the chat manager only if this is the study game.
        bool feedbackSent = false;
        if(!this.isTestGame)
        {
            feedbackSent = Application.Instance.chatManager.SendFeedback();
        }
        // if no feedback has been sent, we directly go to the next state
        if(!feedbackSent)
        {
            SetState(GameStates.ProcessPlayerInput);
        }
        // otherwise, we will go to the next state when the user clicks confirm 
    }

    private void ProcessPlayerInput()
    {
        Application app = Application.Instance;
        int contribution = app.menuController.ProcessContribution();
        app.theWorld.humanPlayer.SetContribution(contribution);
        
        SetState(GameStates.WaitingForOtherPlayers);
    }

    IEnumerator PlayArtificialPlayersRound()
    {
        Application app = Application.Instance;
        if(!isTestGame)
        {
            // Faking the other players
            app.menuController.ActivatePopup("Waiting for other players");
        
            int timeToWait = random.Next(2, 5);
        
        
            yield return new WaitForSeconds(timeToWait);
            app.menuController.DeactivatePopup();

        }

        // calculate the other players' contribution
        foreach(Player player in app.theWorld.activePlayers)
        {
            player.CalculateContribution();
        }

        // go to the PerformPestControl state
        SetState(GameStates.PerformingPestControl);
    }

    IEnumerator PerformPestControl()
    {
        Application app = Application.Instance;
        World theWorld = app.theWorld;
        int totalContribution = 0;
        foreach(Player player in theWorld.activePlayers)
        {
            totalContribution = totalContribution + player.GetContribution();
            Debug.Log("Player " + player.id + " paid " + player.GetContribution());
        }

        double threshold = GetSpreadingThreshold(totalContribution);
        double probaSpread = (1 - threshold) * 100;

        app.chatManager.SendLogMessage("The collective gathered " + totalContribution + " coins, the pest has " + Math.Round(probaSpread) + "% chances to spread");
        app.menuController.ActivatePopup("Performing Pest Control");
        yield return new WaitForSeconds(3);
        app.menuController.DeactivatePopup();

        GridTile pestTile;
        switch(theWorld.pestProgression.type)
        {
            case "random":
            if(PestHasProgressed(totalContribution))
            {
                pestTile = theWorld.GetNextPestTileRandom();
                theWorld.SpawnPestTile(pestTile);
            }
            break;

            case "scripted": 
            pestTile = theWorld.GetNextPestTileScripted();
            if(pestTile == null)
            {
                theWorld.pestProgression.latestPestControlSuccess = true;
            }
            else
            {
                theWorld.pestProgression.latestPestControlSuccess = false;
            }

            theWorld.SpawnPestTile(pestTile);                    
            break;

            case "semiscripted":
            if(PestHasProgressed(totalContribution))
            {
                pestTile = theWorld.GetNextPestTileSemiScripted();
                theWorld.SpawnPestTile(pestTile);
            }
            break;

            default: 
            Debug.LogError("Pest progression type " + theWorld.pestProgression.type + " unknown. Known types are (random, scripted, semiscripted)");
            break;
        }
        // update the game board status
        Application.Instance.gameBoardController.GameBoardChanged();

        // Check if the player lost the game
        if(!theWorld.activePlayers.Contains(theWorld.humanPlayer))
        {
            LoseGame();
        }
        if(currentGameState != GameStates.GameEnded)
        {
            SetState(GameStates.ConfirmPestControl);
        }


    }

    private void ConfirmPestControl()
    {
        if(Application.Instance.theWorld.pestProgression.latestPestControlSuccess)
        {
            Application.Instance.chatManager.SendLogMessage("The Pest Control was successful");
        }
        else
        {
            Application.Instance.chatManager.SendLogMessage("The Pest Control was unsuccessful");
        }
        SetState(GameStates.CollectRevenue);
    }

    private void CollectRevenue()
    {
        foreach(Player player in Application.Instance.theWorld.activePlayers)
        {
            player.CollectRevenue();
        }
        Application.Instance.chatManager.SendLogMessage("You've earned " + Application.Instance.theWorld.humanPlayer.revenuePerYear + " coins from your farm.");
        SetState(GameStates.PrepareForNextYear);
    } 

    private void PrepareForNextYear()
    {
        World theWorld = Application.Instance.theWorld;
        if(theWorld.currentYear == theWorld.maxYear)
        {
            WinGame();
        }

        if(currentGameState != GameStates.GameEnded)
        {
            theWorld.currentYear = theWorld.currentYear + 1;
            SetState(GameStates.WaitingForPlayerInput);
        }
    }

    ///////////////////////////////////////////////////////////////////// Private functions used by the State Machine
    IEnumerator WaitForOtherPlayersToConnect()
    {
        Debug.Log("Waiting for other players to connect");
        // display connection window
        Application.Instance.menuController.DeactivateMenu();
        Application.Instance.menuController.ActivateConnectionPopup();

        yield return new WaitForSeconds(10);

        Application.Instance.menuController.DeactivateConnectionPopup();

        SetState(GameStates.WaitingForPlayerInput);
    }


    private bool PestHasProgressed(int totalContribution)
    {
        double threshold = GetSpreadingThreshold(totalContribution);
        double p = random.NextDouble();
        Debug.Log("p: " + p + ", threshold: " + threshold);
        
        if(p < threshold)
        {
            Application.Instance.theWorld.pestProgression.latestPestControlSuccess = true;
            return false;
        }
        else
        {  
            Application.Instance.theWorld.pestProgression.latestPestControlSuccess = false;
            return true;  
        }   
    }

    private double GetSpreadingThreshold(int totalContribution)
    {
        World theWorld = Application.Instance.theWorld;
        double threshold = (theWorld.easeOfPestControl * totalContribution) / (1 + theWorld.easeOfPestControl * totalContribution);
        return threshold;
    }

    private void WinGame()
    {
        currentGameState = GameStates.GameEnded;
        Application.Instance.menuController.ActivateEndGamePopup("CONGRATULATIONS \n You have reached the end of the game");
    }

    private void LoseGame()
    {
        currentGameState = GameStates.GameEnded;
        Application.Instance.menuController.ActivateEndGamePopup("GAME OVER \nThe pest has reached your farm");
    }



    ///////////////////////////////////////////////////////////////////// Methods called by UI controllers
    public void Paid()
    {
        Debug.Log("PestGameManager.Paid");
        SetState(GameStates.ConfirmPlayerInput); // we go to the confirm state
    }

    public void ActionConfirmed()
    {
        SetState(GameStates.ProcessPlayerInput); // we go to the "waiting for other players" state
    }

    public void ActionCancelled()
    {
        SetState(GameStates.WaitingForPlayerInput);
    }

    public void EndGameClicked()
    {
        Application.Instance.protocolManager.GameFinished();
    }
}