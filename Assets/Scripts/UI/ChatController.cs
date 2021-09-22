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
    private GameObject agentPanel;

    [SerializeField]
    private GameObject agent;

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

    [SerializeField]
    private Text successMessage;

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
        //PestApplication.Instance.gameManager.ActionConfirmed();
        participantAnswerSection.SetActive(false);
        PestApplication.Instance.menuController.ActivateMenu();
        DectivateAgentPanel();
        PestApplication.Instance.chatManager.PlayerInputConfirmed();
    }

    public void CancelInput()
    {
        PestApplication.Instance.gameManager.ActionCancelled();
        participantAnswerSection.SetActive(false);
        DectivateAgentPanel();
    }

    public void ActivateAnswerSection()
    {
        PestApplication.Instance.menuController.SetTalkingRobot();
        participantAnswerSection.SetActive(true);
    }

    public void ActivateAgentPanel()
    {
        PestApplication.Instance.menuController.DeactivateMenu();
        agentPanel.SetActive(true);
    }

    public void DectivateAgentPanel()
    {
        PestApplication.Instance.menuController.ActivateMenu();
        PestApplication.Instance.menuController.SetNeutralRobot();
        agentPanel.SetActive(false);
    }

    public void ToggleAgentPanel()
    {
        if (!participantAnswerSection.activeSelf && messageList.Count > 0)
        {
            if (agentPanel.activeSelf)
            {
                DectivateAgentPanel();
            }
            else
            {
                ActivateAgentPanel();
            }
        }
    }


    /*** SUMMARY ***/


    public void ActivateSummary(int ncoins, double pspreading, int year)
    { 
        coinsCollected.text = ncoins.ToString();
        probabilitySpreading.text = ((int)pspreading).ToString();
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

        if (PestApplication.Instance.theWorld.pestProgression.latestPestControlSuccess)
        {
             successMessage.text = "The Pest Control was <color=#48a7c7>successful</color>!";
        }
        else
        {
            successMessage.text= "The Pest Control was <color=red>unsuccessful</color>.";
        }
    }

    public void DeactivateSummary()
    {
        summaryPanel.SetActive(false);
    }

    public void ActivateAgent()
    {
        agent.SetActive(true);
    }
    public void DeactivateAgent()
    {
        agent.SetActive(false);
    }


}