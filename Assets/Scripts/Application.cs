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
    public StartGameController startGmaenController {get;}

    ///////////////////////////////////////// Model objects
    public World theWorld {get;} = null;

    //////////////////////////////////////////////Contructior
    Application()
    {

        // init the world
        string worldJson = Resources.Load<TextAsset>(@"Config/world").text;
        this.theWorld = JsonConvert.DeserializeObject<World>(worldJson);
       
        theWorld.Init();

        chatManager = GameObject.Find("Managers").GetComponent<ChatManager>();
        gameManager = GameObject.Find("Managers").GetComponent<PestGameManager>();
        tutorialManager = GameObject.Find("Managers").GetComponent<TutorialManager>();
        protocolManager = GameObject.Find("Managers").GetComponent<ProtocolManager>();


        chatController = GameObject.Find("ChatSection").GetComponent<ChatController>();
        menuController = GameObject.Find("MenuSection").GetComponent<MenuController>();
        tutorialController = GameObject.Find("TutorialSection").GetComponent<TutorialController>();
        gameBoardController = GameObject.Find("GameBoardSection").GetComponent<GameBoardController>();
        startGmaenController = GameObject.Find("StartGameSection").GetComponent<StartGameController>();
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
}
