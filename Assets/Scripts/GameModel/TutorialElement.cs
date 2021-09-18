using UnityEngine;
using UnityEngine.UI;
public class TutorialElement
{
    public GameObject uiObject {get; private set;}
    public Location tile {get; private set;}

    public string explanation {get; private set;}

    public TutorialElement(string message, GameObject gameObject)
    {
        this.uiObject = gameObject;
        this.tile = null;
        this.explanation = message;
    }

    public TutorialElement(string message, Location tile)
    {
        this.uiObject = null;
        this.tile = tile;
        this.explanation = message;
    }
    
    public TutorialElement(string message)
    { 
        this.uiObject = null;
        this.tile = null;
        this.explanation = message;
    }
}