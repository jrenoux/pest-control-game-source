using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

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
        logManager = GameObject.Find("Managers").GetComponent<DataLogManager>();

        // init the test world
        string testWorldJson = Resources.Load<TextAsset>(@"Config/test-world").text;
        this.testWorld = JsonConvert.DeserializeObject<World>(testWorldJson);

        // init the study world
        string studyWorldJson = Resources.Load<TextAsset>(@"Config/study-world").text;
        this.studyWorld = JsonConvert.DeserializeObject<World>(studyWorldJson);
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

    public long GetCurrentTimestamp()
    {
        DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
        long timestamp = now.ToUnixTimeMilliseconds(); // 1565642183

        return timestamp;
    }
}
