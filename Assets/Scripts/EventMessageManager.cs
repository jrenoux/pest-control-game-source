using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventMessageManager : MonoBehaviour
{
    public Text messageText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CoachSays(string message)
    {
        messageText.text = message;
    }
}
