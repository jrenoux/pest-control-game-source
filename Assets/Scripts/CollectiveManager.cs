using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectiveManager : MonoBehaviour
{      
    Text[] playerText;
    Text[] contributionText;
    int[] contributionPerPlayer;
    Text totalText;
    Text totalContributionText;

    private GameController gameController;
    private int nbPlayers = 0;

    public Text prefabPlayerText;
    public Text prefabContributionText; 

    bool isReady = false;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
        StartCoroutine(InitNumberOfPlayers());
    }

    // Update is called once per frame
    void Update()
    {
       StartCoroutine(UpdateView());
    }

    private IEnumerator UpdateView()
    {
        yield return new WaitUntil(() => isReady);
        //update total contribution
        int totalContribution = 0;
        for(int i = 0; i < nbPlayers ; i++)
        {
            totalContribution = totalContribution + contributionPerPlayer[i];
            if(gameController.GetCoachIcLevel() == GameController.InfoCollective.Full) 
            {
                contributionText[i].text = contributionPerPlayer[i].ToString();
            }
            
        }
        if(gameController.GetCoachIcLevel() == GameController.InfoCollective.Full || gameController.GetCoachIcLevel() == GameController.InfoCollective.Total)
        {
            totalContributionText.text = totalContribution.ToString();
        }  
        
    }

    public IEnumerator InitNumberOfPlayers()
    {
        yield return new WaitUntil(() => gameController.IsReady());

        nbPlayers = gameController.GetNbPlayers();

        playerText = new Text[nbPlayers];
        contributionText = new Text[nbPlayers];
        contributionPerPlayer = new int[nbPlayers];


        if(gameController.GetCoachIcLevel() == GameController.InfoCollective.None)
        {
            GameObject.Find("CollectiveSection").SetActive(false);
        }
        else
        {
            // Only displays the contribution of each player if the coach info level is full

            if(gameController.GetCoachIcLevel() == GameController.InfoCollective.Full) 
            {
                for(int i = 0 ; i < nbPlayers ; i++) 
                {
                    playerText[i] = Instantiate(prefabPlayerText, transform);
                    playerText[i].text = "Player " + i;
                    contributionText[i]  = Instantiate(prefabContributionText, transform);
                    contributionText[i].text = "0";
                    if(i == gameController.GetActivePlayerId()) 
                    {
                        playerText[i].text = "You";
                    }
                }
            }
            
            // Only displayes the total contribution if the coach info level is full or total
            if(gameController.GetCoachIcLevel() == GameController.InfoCollective.Total || gameController.GetCoachIcLevel() == GameController.InfoCollective.Full) 
            {
                totalText = Instantiate(prefabPlayerText, transform);
                totalText.text = "Total";
                totalContributionText = Instantiate(prefabContributionText, transform);
                totalContributionText.text = "0";
            }
        }

        
        
        isReady = true;
    }

    public void ChangeContribution(int playerID, int contribution) 
    {
        contributionPerPlayer[playerID] = contribution;
    }
}
