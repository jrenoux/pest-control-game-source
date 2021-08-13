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
    private string sessionId;
    [JsonProperty]
    private string gameType;

    public DataEntryGameConfig(string prolificId, string sessionId, string gameType, World theWorld)
    {
        this.prolificId = prolificId;
        this.sessionId = sessionId;
        this.theWorld = theWorld;
        this.gameType = gameType;
    }

}
