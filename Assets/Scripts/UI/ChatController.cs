using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ChatController : MonoBehaviour
{
    [SerializeField]
    private GameObject messageTemplate;
    
    [SerializeField]
    private GameObject participantAnswerSection;

    [SerializeField]
    private GameObject summaryPanel;

    [SerializeField]
    private Text coinsCollected;

    [SerializeField]
    private Text probabilitySpreading;

    [SerializeField]
    private Text previousYear;

    [SerializeField]
    private Image riskImage;

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
        PestApplication.Instance.gameManager.ActionConfirmed();
        participantAnswerSection.SetActive(false);
    }

    public void CancelInput()
    {
        PestApplication.Instance.gameManager.ActionCancelled();
        participantAnswerSection.SetActive(false);
    }

    public void ActivateAnswerSection()
    {
        participantAnswerSection.SetActive(true);
    }


    /*** SUMMARY ***/
    

    public void ActivateSummary(int ncoins, double pspreading, int year)
    { 
        coinsCollected.text = ncoins.ToString();
        probabilitySpreading.text = pspreading.ToString();
        previousYear.text = year.ToString();
        summaryPanel.SetActive(true);

        switch (pspreading)
        {
            case double n when n >= 80.0:

                riskImage.sprite = Resources.Load<Sprite>("Sprites/bad");
                break;
            case double n when n >= 60.0:
                riskImage.sprite = Resources.Load<Sprite>("Sprites/poor");
                break;
            case double n when n >= 50.0:
                riskImage.sprite = Resources.Load<Sprite>("Sprites/fair");
                break;
            case double n when n < 50.0:
                riskImage.sprite = Resources.Load<Sprite>("Sprites/good");
                break;
            case double n when n <= 10.0:
                riskImage.sprite = Resources.Load<Sprite>("Sprites/excelent");
                break;
        }
    }

    public void DeactivateSummary()
    {
        summaryPanel.SetActive(false);
    }


}