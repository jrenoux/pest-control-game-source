using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DataEntryGameConfig
{
    [JsonProperty]
    World theWorld;

    [JsonProperty]
    private string prolificId;
    [JsonProperty]
    public string sessionId {get; private set;}
    [JsonProperty]
    private string gameType;
    [JsonProperty]
    private string condition;
    [JsonProperty]
    private long startTutorialTimestamp;
    [JsonProperty]
    private long startGameTimestamp;
    [JsonProperty]
    private List<long> tutorialClicksTimestamps; 

    public DataEntryGameConfig(string prolificId, string sessionId, string condition, string gameType, World theWorld)
    {
        this.prolificId = prolificId;
        this.sessionId = sessionId;
        this.condition = condition;
        this.theWorld = theWorld;
        this.gameType = gameType;
        this.tutorialClicksTimestamps = new List<long>();
    }

    public void SetStartTutorialTimestamp(long startTutorialTimestamp)
    {
        this.startTutorialTimestamp = startTutorialTimestamp;
    }

    public void SetStartGameTimestamp(long startGameTimestamp)
    {
        this.startGameTimestamp = startGameTimestamp;
    }

    public void AddTutorialClickTimestamp(long tutorialClickTimestamp)
    {
        this.tutorialClicksTimestamps.Add(tutorialClickTimestamp);
    }





}
