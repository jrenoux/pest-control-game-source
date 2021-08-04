using UnityEngine;
using System.Collections.Generic;

using Newtonsoft.Json;

public sealed class Application
{
    //////////////////////////////////////////////Singleton attributes
    private static Application instance = null;
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

    //////////////////////////////////////////////Contructior
    Application()
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
       
    }

    /////////////////////////////////////////////Singleton method
    public static Application Instance
    {
        get
        {
            lock(padlock)
            {
                if (instance == null)
                {
                    instance = new Application();
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
    }

    public void SetupStudyGame()
    {
        // init the study world
        // init the world
        string studyWorldJson = Resources.Load<TextAsset>(@"Config/study-world").text;
        this.studyWorld = JsonConvert.DeserializeObject<World>(studyWorldJson);
       
        studyWorld.Init();

        theWorld = studyWorld;
        gameBoardController.GameBoardChanged();
    }
}
