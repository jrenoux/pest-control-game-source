using System.Collections.Generic;
using UnityEngine;
public class Feedback
{   
    
    public string condition {get; set;}
    public List<FeedbackItem> feedbackItems {get; set;}
    public string GetFeedbackUtterance(int roundNumber, GridTile pestTile)
    {
        Debug.Log(roundNumber);
        Debug.Log(pestTile);
        // TODO 
        List<string> possibleFeedbacks = new List<string>();
        foreach(FeedbackItem item in feedbackItems)
        {
            Debug.Log(item);
            if(item.round == roundNumber 
            || 
            (item.pest != null 
            && pestTile != null
            && item.pest.Equals(new Location(pestTile.coordinates))))
            {
                possibleFeedbacks.Add(item.utterance);
            } 
        }

        if (possibleFeedbacks.Count == 0)
        {
            return "";
        }

        int randIndex = RandomSingleton.GetInstance().Next(0, possibleFeedbacks.Count);
        return possibleFeedbacks[randIndex];
    }
}

public class FeedbackItem
{
    public int round {get; set;} = -1;
    public Location pest {get; set;} = null;
    public string utterance {get; set;}
}