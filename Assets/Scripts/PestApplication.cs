using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

public sealed class PestApplication
{
    //////////////////////////////////////////////Singleton attributes
    private static PestApplication instance = null;
    private static readonly object padlock = new object();

    //////////////////////////////////////////////Game attributes
    ///////////////////////////////////////// Managers
    public PestGameManager gameManager {get;}
    public TutorialManager tutorialManager {get;}
    public ProtocolManager protocolManager {get;}
    public ChatManager chatManager {get;}

    ///////////////////////////////////////// UI controllers
    public ChatController chatController {get;}
    public MenuController menuController {get;}
    public GameBoardController gameBoardController {get;}
    public TutorialController tutorialController {get;}
    public StartGameController startGameController {get;}

    public CharityPopupController charityController {get;}

    ///////////////////////////////////////// Model objects
    public World theWorld {get; set;} = null;

    public World testWorld {get; private set;}
    public World studyWorld {get; private set;}


    ///////////////////////////////////////// Other objects
    public string prolificID {get; set;}
    public bool debug {get;} = true;

    public  DataLogManager logManager {get;}
    public DataEntryGameConfig gameConfig {get; private set;}


    //////////////////////////////////////////////Contructior
    PestApplication()
    {
        chatManager = GameObject.Find("Managers").GetComponent<ChatManager>();
        gameManager = GameObject.Find("Managers").GetComponent<PestGameManager>();
        tutorialManager = GameObject.Find("Managers").GetComponent<TutorialManager>();
        protocolManager = GameObject.Find("Managers").GetComponent<ProtocolManager>();


        chatController = GameObject.Find("ChatSection").GetComponent<ChatController>();
        menuController = GameObject.Find("MenuSection").GetComponent<MenuController>();
        tutorialController = GameObject.Find("TutorialSection").GetComponent<TutorialController>();
        gameBoardController = GameObject.Find("GameBoardSection").GetComponent<GameBoardController>();
        startGameController = GameObject.Find("StartGameSection").GetComponent<StartGameController>();
        charityController = GameObject.Find("CharityPopup").GetComponent<CharityPopupController>();

        logManager = GameObject.Find("Managers").GetComponent<DataLogManager>();

        // init the test world
        string testWorldJson = Resources.Load<TextAsset>(@"Config/test-world").text;
        this.testWorld = JsonConvert.DeserializeObject<World>(testWorldJson);

        // init the study world
        string studyWorldJson = Resources.Load<TextAsset>(@"Config/study-world").text;
        this.studyWorld = JsonConvert.DeserializeObject<World>(studyWorldJson);

        // retrieve the prolific ID from URL
        string url = Application.absoluteURL;
        prolificID = "";
        Debug.Log("url = " + url);
        if(!url.Equals(""))
        {
            Uri bUri = new Uri(url);

            var query = bUri.Query.Replace("?", "");
            var queryValues = query.Split('&').Select(q => q.Split('='))
                   .ToDictionary(k => k[0], v => v[1]);
            string id;
            if(queryValues.TryGetValue("prolificID", out id)) 
            {
                // we test that it follows the right format
                // prolific IDs contain 24 alphanumerical characters 
                if(ValidateProlificID(id))
                {
                    prolificID = id;
                }
            }
        }
        
        Debug.Log("prolificid = " + prolificID);
    }

    /////////////////////////////////////////////Singleton method
    public static PestApplication Instance
    {
        get
        {
            lock(padlock)
            {
                if (instance == null)
                {
                    instance = new PestApplication();
                }
                return instance;
            }
        }
    }

    public void SetupTestGame()
    {       
        testWorld.Init();

        theWorld = testWorld;
    
		gameConfig = logManager.InitNewGameLog(prolificID, "test", chatManager.feedback.condition, theWorld);

        // Force map draw
        gameBoardController.GameBoardChanged();

    }

    public void SetupStudyGame()
    {
        studyWorld.Init();

        theWorld = studyWorld;

        gameConfig = logManager.InitNewGameLog(prolificID, "study", chatManager.feedback.condition, theWorld);

        // force map redraw
        gameBoardController.GameBoardChanged();
    }

    public DataEntryEndGame EndGame(string gameType, long timestamp, float charityDonation)
    {
        // get the final wallet value
        int finalWallet = theWorld.humanPlayer.wallet;

        Debug.Log("Game finished with " + finalWallet + " coins in wallet");
        Debug.Log("The player donated " + charityDonation.ToString() + " to charity.");

        DataEntryEndGame endGame = new DataEntryEndGame(this.prolificID, logManager.currentSessionId.ToString(),
        gameType, chatManager.feedback.condition, timestamp, finalWallet, charityDonation);

        return endGame;
    }

    public long GetCurrentTimestamp()
    {
        DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
        long timestamp = now.ToUnixTimeMilliseconds(); // 1565642183

        return timestamp;
    }

    public bool ValidateProlificID(string id) 
    {
        var regex = @"^\w{24}$";

            var match = Regex.Match(id, regex, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;        
            }
    }
}
