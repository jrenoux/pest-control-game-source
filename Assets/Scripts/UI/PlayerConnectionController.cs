using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerConnectionController : MonoBehaviour
{
    [SerializeField]
    private Text nbConnectedPlayers;
    
    private int currentNbOfPlayers = 2;
    
    void Awake()
    {
        // deactivate the current object
        transform.gameObject.SetActive(false);
    }
    void OnEnable()
    {
        Debug.Log("On Enable");
        // starts the coroutine
        StartCoroutine(SimulatePlayerConnecting());
    }

    // Update is called once per frame
    void Update()
    {
        nbConnectedPlayers.text = currentNbOfPlayers + " / " + PestApplication.Instance.theWorld.activePlayers.Count;
    }

    private IEnumerator SimulatePlayerConnecting()
    {
        // draw a random number between 2 and 10 
        System.Random random = RandomSingleton.GetInstance();
        int timeToWait = random.Next(2, 10);
        Debug.Log("Waiting for " + timeToWait + " seconds.");

        yield return new WaitForSeconds(timeToWait);

        currentNbOfPlayers = currentNbOfPlayers + 1;

        if(currentNbOfPlayers == PestApplication.Instance.theWorld.activePlayers.Count)
        {
            // wait for just a second
            yield return new WaitForSeconds(1);
            PestApplication.Instance.gameManager.AllPlayerConnected();
        }
        else
        {   
            StartCoroutine(SimulatePlayerConnecting());
        }
    }
}
