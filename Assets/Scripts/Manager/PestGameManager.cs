using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum GameStates 
{
    WaitingToStart,
    WaitingForPlayersToConnect,
    WaitingForPlayerInput,
    ConfirmPlayerInput,
    ProcessPlayerInput,
    WaitingForOtherPlayers,
    InitiatePestControl,
    PerformingPestControl,
    ConfirmPestControl,
    CollectRevenue,
    SendFeedback,
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

    private DataEntryRound roundLog = null;

    private bool allPlayerConnected = false;
    public List<int> collective;

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
                case GameStates.WaitingForPlayersToConnect:
                    Debug.Log("Waiting for players to connect");
                    WaitForOtherPlayersToConnect();
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
                case GameStates.InitiatePestControl:
                    
                    //StartCoroutine(PerformPestControl());
                    Debug.Log("State = Initiate pest control");
                    StartCoroutine(InitiatePestControl());
                    break;
                case GameStates.PerformingPestControl:
                    PerformPestControl();
                    Debug.Log("State = Performing pest control");
                    break;
                case GameStates.ConfirmPestControl:
                    ConfirmPestControl();
                    Debug.Log("State = Confirm pest control status");
                    break;
                case GameStates.CollectRevenue:
                    StartCoroutine(CollectRevenue());
                    Debug.Log("State = Collect revenue");
                    break;
                case GameStates.SendFeedback:
                    SendFeedback();
                    Debug.Log("State = Send Feedback if any");
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
        PestApplication.Instance.chatManager.Reset();
        this.isTestGame = isTestGame;
        allPlayerConnected = false;
        // fake other players' connection
        if(!this.isTestGame)
        {
            SetState(GameStates.WaitingForPlayersToConnect);
        }
        else
        {
            SetState(GameStates.WaitingForPlayerInput);
        }
        World theWorld = PestApplication.Instance.theWorld;
        collective = new List<int>();
        foreach (Player player in theWorld.activePlayers)
        {
            player.contributionsHistory = new List<int>();
        }
    }

    private void WaitForOtherPlayersToConnect()
    {
        // display connection window and deactivate the menu
        PestApplication.Instance.menuController.DeactivateMenu();
        PestApplication.Instance.menuController.ActivateConnectionPopup();
    }

    private void WaitingForPlayerInput()
    {
        PestApplication.Instance.menuController.ActivateMenu();
        // create a new round only if it's a new round (the feedback can bring the player back here)
        if(roundLog == null)
        {
            if(isTestGame)
            {
                roundLog = new DataEntryRound(PestApplication.Instance.prolificID, 
                                            PestApplication.Instance.sessionId,
                                            PestApplication.Instance.theWorld.currentYear, 
                                            PestApplication.Instance.theWorld.tileList);
            }
            else
            {
                roundLog = new DataEntryRound(PestApplication.Instance.prolificID, 
                                            PestApplication.Instance.sessionId,
                                            PestApplication.Instance.theWorld.currentYear, 
                                            PestApplication.Instance.theWorld.tileList);
            }
            
        }
        
    }

    private void ConfirmPlayerInput()
    {
        Debug.Log("PestGameManager.ConfirmPlayerInput");
        // log the amount
        roundLog.SetContribution(PestApplication.Instance.menuController.GetCurrentContribution());
        PestApplication.Instance.menuController.DeactivateMenu();
        // we send feedback through the chat manager only if this is the study game.
        //bool feedbackSent = false;
        //string utterance = "";
        //if(!this.isTestGame)
        //{
        //    (feedbackSent, utterance) = PestApplication.Instance.chatManager.SendFeedback();
        //}
        // if no feedback has been sent, we directly go to the next state
        //if(!feedbackSent)
        //{
            SetState(GameStates.ProcessPlayerInput);
        //}
        // otherwise, we will go to the next state when the user clicks confirm 
        // we just need to log the utterance

        //else
        //{
        //    roundLog.SetCoachUtterance(utterance);
        //}
        
    }

    private void ProcessPlayerInput()
    {
        PestApplication app = PestApplication.Instance;
        int contribution = app.menuController.ProcessContribution();
        app.theWorld.humanPlayer.SetContribution(contribution);
        
        SetState(GameStates.WaitingForOtherPlayers);
    }

    IEnumerator PlayArtificialPlayersRound()
    {
        PestApplication app = PestApplication.Instance;
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
            int c = player.CalculateContribution();
            this.roundLog.AddArtificialPlayersContribution(player.id, c);
        }


        // go to the PerformPestControl state
        SetState(GameStates.InitiatePestControl);
    }

    IEnumerator InitiatePestControl()
    {
        PestApplication app = PestApplication.Instance;
        World theWorld = app.theWorld;
        int totalContribution = 0;
        foreach (Player player in theWorld.activePlayers)
        {
            totalContribution = totalContribution + player.GetContribution();
            Debug.Log("Player " + player.id + " paid " + player.GetContribution());
        }

        collective.Add(totalContribution);
        double threshold = GetSpreadingThreshold(totalContribution);
        double probaSpread = (1 - threshold) * 100;

        app.menuController.ActivatePestControlPopUp();
        
        //app.chatController.ActivateSummary(totalContribution, probaSpread);

        /* app.chatManager.SendLogMessage("The collective gathered " + totalContribution + " coins, the pest has " + Math.Round(probaSpread) + "% chances to spread");
         app.menuController.ActivatePopup("Performing Pest Control");*/
        yield return new WaitForSeconds(2);
        app.menuController.DeactivatePestControlPopUp();

        SetState(GameStates.PerformingPestControl);

    }

    private void PerformPestControl()
    {
        

        PestApplication app = PestApplication.Instance;
        World theWorld = app.theWorld;
        GridTile pestTile;
        int totalContribution = 0;
        foreach (Player player in theWorld.activePlayers)
        {
            totalContribution = totalContribution + player.GetContribution();
            Debug.Log("Player " + player.id + " paid " + player.GetContribution());
        }

        switch (theWorld.pestProgression.type)
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
        PestApplication.Instance.gameBoardController.GameBoardChanged();

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
        PestApplication app = PestApplication.Instance;

        if (PestApplication.Instance.theWorld.pestProgression.latestPestControlSuccess)
        {
            app.menuController.ChangePestControlResult("pest", "The Pest Control was <color=#48a7c7>successful</color>!");
            //PestApplication.Instance.chatManager.SendLogMessage("The Pest Control was successful");
        }
        else
        {
            app.menuController.ChangePestControlResult("pest","The Pest Control was <color=red>unsuccessful</color>.");
            //PestApplication.Instance.chatManager.SendLogMessage("The Pest Control was unsuccessful");
        }
        SetState(GameStates.CollectRevenue);
    }

    IEnumerator CollectRevenue()
    {

        int totalContribution = 0;
        

        foreach (Player player in PestApplication.Instance.theWorld.activePlayers)
        {
            totalContribution = totalContribution + player.GetContribution();
            Debug.Log("Player " + player.id + " paid " + player.GetContribution());
            player.CollectRevenue();
        }
        PestApplication.Instance.menuController.ChangePestControlResult("earnings", "You've earned " + PestApplication.Instance.theWorld.humanPlayer.revenuePerYear + " coins from your farm.");
        //PestApplication.Instance.chatManager.SendLogMessage("You've earned " + PestApplication.Instance.theWorld.humanPlayer.revenuePerYear + " coins from your farm.");

        double threshold = GetSpreadingThreshold(totalContribution);
        double probaSpread = (1 - threshold) * 100;

        PestApplication.Instance.chatController.ActivateSummary(totalContribution, probaSpread, PestApplication.Instance.theWorld.currentYear);

        PestApplication.Instance.menuController.ActivatePestControlResult();
        yield return new WaitForSeconds(3);
        PestApplication.Instance.menuController.DeactivatePestControlResultPopUp();

        

    }


    private void SendFeedback()
    {

        // SEND Feedback Here - After Round finished
        // we send feedback through the chat manager only if this is the study game.
        bool feedbackSent = false;
        string utterance = "";

        if (!this.isTestGame)
        {
            (feedbackSent, utterance) = PestApplication.Instance.chatManager.SendFeedback();
        }
        if (feedbackSent)
        {
            roundLog.SetCoachUtterance(utterance);
        }
        else
        {
            PestApplication.Instance.gameManager.StartNewYear();
        }

    }

    private void PrepareForNextYear()
    {
        World theWorld = PestApplication.Instance.theWorld;
        if(theWorld.currentYear == theWorld.maxYear)
        {
            WinGame();
        }


        if (currentGameState != GameStates.GameEnded)
        {
            theWorld.currentYear = theWorld.currentYear + 1;
            SetState(GameStates.WaitingForPlayerInput);
        }
        // save the log and reset it
        PestApplication.Instance.logManager.SaveRound(roundLog);
        roundLog = null;


        



    }

    ///////////////////////////////////////////////////////////////////// Private functions used by the State Machine
    


    private bool PestHasProgressed(int totalContribution)
    {
        double threshold = GetSpreadingThreshold(totalContribution);
        double p = random.NextDouble();
        Debug.Log("p: " + p + ", threshold: " + threshold);
        
        if(p < threshold)
        {
            PestApplication.Instance.theWorld.pestProgression.latestPestControlSuccess = true;
            return false;
        }
        else
        {  
            PestApplication.Instance.theWorld.pestProgression.latestPestControlSuccess = false;
            return true;  
        }   
    }

    private double GetSpreadingThreshold(int totalContribution)
    {
        World theWorld = PestApplication.Instance.theWorld;
        double threshold = (theWorld.easeOfPestControl * totalContribution) / (1 + theWorld.easeOfPestControl * totalContribution);
        return threshold;
    }

    private void WinGame()
    {
        currentGameState = GameStates.GameEnded;
        PestApplication.Instance.menuController.ActivateEndGamePopup("CONGRATULATIONS \n You have reached the end of the game");
        PestApplication.Instance.chatController.DeactivateSummary();
    }

    private void LoseGame()
    {
        currentGameState = GameStates.GameEnded;
        PestApplication.Instance.menuController.ActivateEndGamePopup("GAME OVER \nThe pest has reached your farm");
        PestApplication.Instance.chatController.DeactivateSummary();
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
        PestApplication.Instance.protocolManager.GameFinished();
    }

    public void AllPlayerConnected()
    {
        PestApplication.Instance.menuController.DeactivateConnectionPopup();

        SetState(GameStates.WaitingForPlayerInput);
    }

    public void StartPestControl()
    {
        SetState(GameStates.PerformingPestControl);
    }

    public void ShowFeedback()
    {
        SetState(GameStates.SendFeedback);
    }

    public void StartNewYear()
    {
        SetState(GameStates.PrepareForNextYear);
    }
        

}