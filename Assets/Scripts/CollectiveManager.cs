using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectiveManager : MonoBehaviour
{      
    Text[] playerText;
    Text[] contributionText;

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
       StartCoroutine(UpdateTotal());
    }

    private IEnumerator UpdateTotal()
    {
        yield return new WaitUntil(() => isReady);
        //update total contribution
        int totalContribution = 0;
        for(int i = 0; i < nbPlayers ; i++)
        {
            totalContribution = totalContribution + int.Parse(contributionText[i].text);
        }
        totalContributionText.text = totalContribution.ToString();
    }

    public IEnumerator InitNumberOfPlayers()
    {
        yield return new WaitUntil(() => gameController.IsReady());

        nbPlayers = gameController.GetNbPlayers();

        playerText = new Text[nbPlayers];
        contributionText = new Text[nbPlayers];

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
        totalText = Instantiate(prefabPlayerText, transform);
        totalText.text = "Total";
        totalContributionText = Instantiate(prefabContributionText, transform);
        totalContributionText.text = "0";

        isReady = true;
    }

    public void ChangeContribution(int playerID, int contribution) 
    {
        contributionText[playerID].text = contribution.ToString();
    }
}
