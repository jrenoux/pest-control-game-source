using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ChatController : MonoBehaviour
{
    [SerializeField]
    private GameObject messageTemplate;
    
    [SerializeField]
    private GameObject participantAnswerSection;

    private List<GameObject> messageList;

    public void Start()
    {
        participantAnswerSection.SetActive(false);
        messageList = new List<GameObject>();
    }


    public void Update()
    {

    }

    public void DisplayChatMessage(string messageText, MessageTypes messageType)
    {
        GameObject message = Instantiate(messageTemplate) as GameObject;
        message.GetComponent<MessageTemplateController>().SpawnChatMessage(messageText, messageType);
        message.SetActive(true);
        message.transform.SetParent(messageTemplate.transform.parent, false);
        messageList.Add(message);
    }

    public void EmptyMessageList()
    {
        foreach(GameObject message in messageList)
        {
            Destroy(message);
        }
    }

    public void ConfirmInput()
    {
        Application.Instance.gameManager.ActionConfirmed();
        participantAnswerSection.SetActive(false);
    }

    public void CancelInput()
    {
        Application.Instance.gameManager.ActionCancelled();
        participantAnswerSection.SetActive(false);
    }

    public void ActivateAnswerSection()
    {
        participantAnswerSection.SetActive(true);
    }

}