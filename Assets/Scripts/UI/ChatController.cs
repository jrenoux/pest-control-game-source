using UnityEngine;
using UnityEngine.UI;
public class ChatController : MonoBehaviour
{
    [SerializeField]
    private GameObject messageTemplate;
    
    [SerializeField]
    private GameObject participantAnswerSection;

    public void Start()
    {
        participantAnswerSection.SetActive(false);
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