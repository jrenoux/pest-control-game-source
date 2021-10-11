using Newtonsoft.Json;

public class DataEntryEndGame
{
    [JsonProperty]
    private string prolificId;
    [JsonProperty]
    private string sessionId;
    [JsonProperty]
    private string gameType;
    [JsonProperty]
    private string condition;
    [JsonProperty]
    private long endGameTimestamp;
    [JsonProperty]
    private int finalWallet;

    public DataEntryEndGame(string prolificId, string sessionId, 
    string gameType, string condition, long endGameTimestamp, int finalWallet)
    {
        this.prolificId = prolificId;
        this.sessionId = sessionId;
        this.condition = condition;
        this.gameType = gameType;
        this.endGameTimestamp = endGameTimestamp;
        this.finalWallet = finalWallet;
    }
}
