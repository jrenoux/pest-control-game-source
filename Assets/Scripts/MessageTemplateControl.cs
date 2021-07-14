using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum MessageTypes {
            LOG,
            COACH,
            PLAYER
}


public class MessageTemplateControl : MonoBehaviour
{

    
    Color32 playerColor = new Color32(217, 91, 113, 255);
    Color32 coachColor = new Color32(255, 255, 255, 100);
    Color32 logColor = new Color32(60, 56, 62, 100);
    
    [SerializeField]
    private Text myText; 

    public void SpawnChatMessage(string textString, MessageTypes messageType)
    {
        myText.text = textString;
        switch(messageType)
        {
            case MessageTypes.LOG:
            GetComponentInChildren<Image>().color = logColor;
            myText.color = Color.white;
            break;

            case MessageTypes.COACH:
            GetComponentInChildren<Image>().color = coachColor;
            break;

            case MessageTypes.PLAYER:
            GetComponentInChildren<Image>().color = playerColor;
            break;

            default:
            break;  

        }
        
    }

    
}
