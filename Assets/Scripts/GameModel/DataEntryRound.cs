using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class DataEntryRound 
{
    [JsonProperty]
    private int roundNumber;
    [JsonProperty]
    private int contribution;// the contribution paid by the player (before or without feedback)
    [JsonProperty]
    private int contributionAfterFeedback; // the contribution paid after feedback
    [JsonProperty]  
    private string coachUtterance; // the feedback sent by the agent
    [JsonProperty]
    private bool receivedFeedback;
    [JsonProperty]
    private Dictionary<string, int> artificialPlayerContributions; // what the artificial players paid
    [JsonProperty]
    private List<string> map; // what the map looks like at this round
    [JsonProperty]
    private string userId;
    [JsonProperty]
    private string sessionId;

    public DataEntryRound(string userId, string sessionId, int roundNumber, HashSet<GridTile> currentMapState) 
    {
        this.userId = userId;
        this.sessionId = sessionId;
        this.roundNumber = roundNumber;
        this.contribution = -1;
        this.contributionAfterFeedback = -1;
        this.coachUtterance = "";
        this.artificialPlayerContributions = new Dictionary<string, int>();
        this.receivedFeedback = false;
        // clone the map
        this.map = new List<string>();
        foreach(GridTile tile in currentMapState)
        {
            map.Add(tile.ToString());
        }
                
    }

    public void SetContribution(int contribution)
    {
        if(this.contribution == -1)
        {
            // no contribution has been set yet
            this.contribution = contribution;
        }
        else
        {
            // one contribution has been set already, so this is after the feedback
            this.contributionAfterFeedback = contribution;
        }
    }

    public void SetCoachUtterance(string utterance)
    {
        this.coachUtterance = utterance;
        this.receivedFeedback = true;
    }

    public void AddArtificialPlayersContribution(string player, int contribution)
    {
        artificialPlayerContributions.Add(player, contribution);
    }



}
