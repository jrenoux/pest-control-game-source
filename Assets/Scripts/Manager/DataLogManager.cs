using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;

public class DataLogManager : MonoBehaviour
{
    public System.Guid currentSessionId {get; private set;}
    public DataEntryGameConfig InitNewGameLog(string prolificId, string gameType, string condition, World theWorld)
    {
        // setup the config information
        // create a new session ID
        currentSessionId = System.Guid.NewGuid();
        // serialize and stores the game config info
        DataEntryGameConfig gameConfig = new DataEntryGameConfig(prolificId, currentSessionId.ToString(), condition, gameType, theWorld);

        // returns the gameConfig object
        return gameConfig;

    }

    public void SaveGameConfig(DataEntryGameConfig config)
    {
        // store in DB
        string entityJson = JsonConvert.SerializeObject(config);
        StartCoroutine(Upload(entityJson, result => {
            Debug.Log("MongoDB New Game Upload: " + result);
        }));

        // for debug we print it
        Debug.Log(entityJson);

    }

    public void SaveRound(DataEntryRound round)
    {
        // stores the round in the DB
        string entityJson = JsonConvert.SerializeObject(round);
        StartCoroutine(Upload(entityJson, result => {
            Debug.Log("MongoDB Save Round Upload: " + result);
            if (!result)
            {
                StartCoroutine(Upload(entityJson, result => {
                    Debug.Log("MongoDB Save Round Upload RETRYING (1): " + result);
                    if (!result)
                    {
                        StartCoroutine(Upload(entityJson, result => {
                            Debug.Log("MongoDB Save Round Upload RETRYING (2): " + result);
                            if (!result)
                            {
                                StartCoroutine(Upload(entityJson, result => {
                                    Debug.Log("MongoDB Save Round Upload RETRYING (3): " + result);
                                }));
                            }
                        }));
                    }
                }));
            }
        }));
        Debug.Log(entityJson);
    }

    public void SaveEndGame(DataEntryEndGame endGame)
    {
        string entityJson = JsonConvert.SerializeObject(endGame);
        StartCoroutine(Upload(entityJson, result => {
            Debug.Log("MongoDB New Game Upload: " + result);
        }));

        // for debug we print it
        Debug.Log(entityJson);
    }

    IEnumerator Upload(string content, System.Action<bool> callback = null)
    {
        using (UnityWebRequest request = new UnityWebRequest("https://eu-central-1.aws.webhooks.mongodb-realm.com/api/client/v2.0/app/pestcontrolgame-dnxqz/service/UnityRequests/incoming_webhook/webhook0", "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(content);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                if (callback != null)
                {
                    callback.Invoke(false);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback.Invoke(request.downloadHandler.text != "{}");
                }
            }
        }
    }
}
