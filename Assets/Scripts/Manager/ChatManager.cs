using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public class ChatManager : MonoBehaviour
{
    private List<string> messageList; // contains all the messages that have been sent to the chat. For logging purposes
    public Feedback feedback {get; set;} // contains the template for the feedback, loaded from json file

    // the feedback condition will be set outside of the game. So we suppose we have a feedback file already containing the right feedback for the right condition

    private int latestRoundFeedbackSent = -1;
    private GridTile latestPestTileFeedbackSent = null;

    public void Awake()
    {
        // load the feedback
        string feedbackJson = Resources.Load<TextAsset>(@"Config/feedback").text;
        feedback = JsonConvert.DeserializeObject<Feedback>(feedbackJson);
        messageList = new List<string>();
    }

    public (bool, string) SendFeedback()
    {
        // only send feedback if feedback condition is on
        if(feedback.condition.Equals("control"))
        {
            return (false, "");
        }

        int roundNumber = PestApplication.Instance.theWorld.currentYear;
        GridTile newestPestTile = PestApplication.Instance.theWorld.pestProgression.latestPestSpread;
        Debug.Log("latestRoundNumber = " + latestRoundFeedbackSent);
        Debug.Log("roundNumber = " + roundNumber);
        Debug.Log("latestPestTile = " + latestPestTileFeedbackSent);
        Debug.Log("pestTile = " + newestPestTile);
        // if feedback for this round or gridTile has already been sent, we don't send it again
        if(roundNumber == latestRoundFeedbackSent || (newestPestTile!= null && newestPestTile.Equals(latestPestTileFeedbackSent)))
        {
            return (false, "");
        }
        //returns true if there has been a chat message sent, false otherwise
        // see if there is a chat message to be sent.
        string utterance = feedback.GetFeedbackUtterance(roundNumber, newestPestTile);

        // if there is, send it
        if(!utterance.Equals(""))
       {
           AddChatMessage(utterance, MessageTypes.COACH);
           if(roundNumber != -1)
           {
               latestRoundFeedbackSent = roundNumber;
           }
           if(newestPestTile != null)
           {
               latestPestTileFeedbackSent = newestPestTile;
           }
           
           PestApplication.Instance.chatController.ActivateAnswerSection();
           return (true, utterance);
       }    

       return (false, "");
    }

    public void SendLogMessage(string message)
    {
        AddChatMessage(message, MessageTypes.LOG);
    }

    private void AddChatMessage(string messageString, MessageTypes messageType)
    {
        messageList.Add(messageString);
        PestApplication.Instance.chatController.DisplayChatMessage(messageString, messageType);
    }

    public void Reset()
    {
        Debug.Log("Resetting the chat");
        messageList.Clear();
        PestApplication.Instance.chatController.EmptyMessageList();
    }

}