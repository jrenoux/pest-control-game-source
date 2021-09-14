using System.Collections.Generic;
using UnityEngine;
public class Feedback
{   
    
    public string condition {get; set;}
    public List<FeedbackItem> feedbackItems {get; set;}
    public string GetFeedbackUtterance(int roundNumber, GridTile pestTile)
    {
        // TODO 
        List<string> possibleFeedbacks = new List<string>();
        foreach(FeedbackItem item in feedbackItems)
        {
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
        string chosenFeedback = possibleFeedbacks[randIndex];
        if (chosenFeedback.Contains("{collectiveAVG}"))
        {
            double collectiveAVG = 0;
            foreach (int value in PestApplication.Instance.gameManager.collective)
            {
                collectiveAVG += value;
            }
            collectiveAVG /= PestApplication.Instance.gameManager.collective.Count;
            chosenFeedback = chosenFeedback.Replace("{collectiveAVG}", ((int) collectiveAVG).ToString());
        }
        if (chosenFeedback.Contains("{farmerContributionAVG}"))
        {
            double collectiveAVG = 0;
            foreach (int value in PestApplication.Instance.gameManager.collective)
            {
                collectiveAVG += value;
            }
            collectiveAVG /= PestApplication.Instance.gameManager.collective.Count;
            //TODO this assumes no farm will be caught during the game
            double farmerContributionAVG = collectiveAVG / PestApplication.Instance.theWorld.activePlayers.Count;
            chosenFeedback = chosenFeedback.Replace("{farmerContributionAVG}", ((int)farmerContributionAVG).ToString());
        }
        return chosenFeedback;
    }
}

public class FeedbackItem
{
    public int round {get; set;} = -1;
    public Location pest {get; set;} = null;
    public string utterance {get; set;}
}