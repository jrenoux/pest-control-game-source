using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public Text prefabPlayerText;
    public Text prefabContributionText;
    Text[] playerText;
    Text[] contributionText;
    Text totalText;
    Text totalContributionText;
    // Start is called before the first frame update

    GameController gameController;
    void Start()
    {
        gameController = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitGrid(int nbPlayers, bool fullInit)
    {
        if(fullInit)
        {
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
        }

        totalText = Instantiate(prefabPlayerText, transform);
        totalText.text = "Total";
        totalContributionText = Instantiate(prefabContributionText, transform);
        totalContributionText.text = "0";
    }

    public void UpdateContributionDisplay(int player, int contribution)
    {
        contributionText[player].text = contribution.ToString();
    }

    public void UpdateTotalDisplay(int totalContribution)
    {
        totalContributionText.text = totalContribution.ToString();
    }


}
