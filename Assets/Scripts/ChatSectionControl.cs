using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
public class ChatSectionControl : MonoBehaviour
{
    [SerializeField]
    private GameObject messageTemplate;
    
    [SerializeField]
    private GameObject participantAnswerSection;

    private List<string> messageList;

    public Feedback feedback {get; set;}

    //TODO this is only for debug. Should change it to configurable
    public bool hasFeedback = true;

    GameControllerScript gameController;

    
    void Start() 
    {
        string feedbackJson = Resources.Load<TextAsset>(@"Config/feedback").text;
        feedback = JsonConvert.DeserializeObject<Feedback>(feedbackJson);
        messageList = new List<string>();
        participantAnswerSection.SetActive(false);
        gameController = GameObject.Find("GameController").GetComponent<GameControllerScript>();
    }
    private void AddChatMessage(string messageString, MessageTypes messageType)
    {
        // TODO add who the message is from
        messageList.Add(messageString);
        GameObject message = Instantiate(messageTemplate) as GameObject;
        message.GetComponent<MessageTemplateControl>().SpawnChatMessage(messageString, messageType);
        message.SetActive(true);
        message.transform.SetParent(messageTemplate.transform.parent, false);
    }

    public bool SendFeedback(int roundNumber, GridTile pestTile)
    {
        //returns true if there has been a chat message sent, false otherwise
        // see if there is a chat message to be sent.
        string utterance = feedback.GetFeedbackUtterance(roundNumber, pestTile);

        // if there is, send it
        if(!utterance.Equals(""))
       {
           AddChatMessage(utterance, MessageTypes.COACH);
           participantAnswerSection.SetActive(true);
           return true;
       }    

       return false;

    }

    public void ConfirmInput()
    {
        // player confirms what they wanted to do, we go to the next state
        gameController.NextState();
        // deactivate the control from the chat 
        participantAnswerSection.SetActive(false);
    }

    public void CancelInput()
    {
        // player cancelled they current input, go back to the waiting player input state
        gameController.SetState(GameControllerScript.GameStates.WaitingForPlayerInput);
        participantAnswerSection.SetActive(false);
    }

}
