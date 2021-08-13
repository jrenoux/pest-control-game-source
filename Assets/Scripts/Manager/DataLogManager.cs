using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

public class DataLogManager : MonoBehaviour
{
    private System.Guid currentSessionId;
    public string InitNewGameLog(string prolificId, string gameType, string condition, World theWorld)
    {
        // setup the config information
        // create a new session ID
        currentSessionId = System.Guid.NewGuid();
        // serialize and stores the game config info
        DataEntryGameConfig gameConfig = new DataEntryGameConfig(prolificId, currentSessionId.ToString(), condition, gameType, theWorld);
        // TODO store in DB

        /******************************************************/
        // TODO
        // MONGODB CONNECTION TO ADD HERE 
        /******************************************************/

        // for now we just print it
        Debug.Log(JsonConvert.SerializeObject(gameConfig));

        // returns the session ID
        return currentSessionId.ToString();

    }

    public void SaveRound(DataEntryRound round)
    {
        // TODO stores the round in the DB
        
        /******************************************************/
        // TODO
        // MONGODB CONNECTION TO ADD HERE 
        /******************************************************/

        
        // for now we just print it
        Debug.Log(JsonConvert.SerializeObject(round));
    }
}
