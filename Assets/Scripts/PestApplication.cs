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

    private World testWorld = null;
    private World studyWorld = null;


    ///////////////////////////////////////// Other objects
    public string prolificID {get; set;}
    public bool debug {get;} = true;

    public  DataLogManager logManager {get;}
    public string sessionId {get; set;}


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
        // init the test world
        string testWorldJson = Resources.Load<TextAsset>(@"Config/test-world").text;
        this.testWorld = JsonConvert.DeserializeObject<World>(testWorldJson);
       
        testWorld.Init();

        theWorld = testWorld;
    
		DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;

		long startTestTimestamp = now.ToUnixTimeMilliseconds(); // 1565642183
        Debug.Log(startTestTimestamp);

        sessionId = logManager.InitNewGameLog(prolificID, "test", chatManager.feedback.condition, startTestTimestamp, theWorld);

        // Force map draw
        gameBoardController.GameBoardChanged();

    }

    public void SetupStudyGame()
    {
        // init the study world
        // init the world
        string studyWorldJson = Resources.Load<TextAsset>(@"Config/study-world").text;
        this.studyWorld = JsonConvert.DeserializeObject<World>(studyWorldJson);
       
        studyWorld.Init();

        theWorld = studyWorld;


        DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;

		long startStudyTimestamp = now.ToUnixTimeMilliseconds(); // 1565642183

        sessionId = logManager.InitNewGameLog(prolificID, "study", chatManager.feedback.condition, startStudyTimestamp, theWorld);

        // force map redraw
        gameBoardController.GameBoardChanged();
    }
}
