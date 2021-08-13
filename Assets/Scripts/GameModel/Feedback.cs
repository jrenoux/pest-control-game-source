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
        bool found = false;
        foreach(FeedbackItem item in feedbackItems)
        {
            Debug.Log(item);
            if(item.round == roundNumber 
            || 
            (item.pest != null 
            && pestTile != null
            && item.pest.Equals(new Location(pestTile.coordinates))))
            {
                return item.utterance;
            } 
        }
        if(!found)
        {
            return "";
        }
        else
        {
            // shold not arrive here
            throw new System.Exception("Should not have reached this point");
            return "";
        }
    }
}

public class FeedbackItem
{
    public int round {get; set;} = -1;
    public Location pest {get; set;} = null;
    public string utterance {get; set;}
}