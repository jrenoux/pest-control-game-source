using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TutorialController : MonoBehaviour 
{
    [SerializeField]
    private GameObject tutorialPopup;
    
    [SerializeField]
    private Image arrowImage;
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private Camera camera;

    public void Start() 
    {
        tutorialPopup.SetActive(false);
    }

    public void Update()
    {
    
    }

    public void DeactivateTutorialPopup()
    {
        tutorialPopup.SetActive(false);
    }

    public void NextButtonClicked()
    {
        Application.Instance.tutorialManager.NextTutorial();
    }

    public void DisplayTutorialPanel(string title, string message, string buttonText,
                                     Vector3 windowPosition, 
                                     Vector3 arrowPosition, Quaternion arrowRotation)
    {
        Text[] texts = tutorialPopup.GetComponentsInChildren<Text>();
        Text tutorialTitle = texts[0];
        Text tutorialMessage = texts[1];
        Text tutorialButtonText = texts[2];
        tutorialTitle.text = title;
        tutorialMessage.text = message;
        tutorialButtonText.text = buttonText;

        tutorialPopup.transform.position = windowPosition;  
        arrowImage.transform.position = arrowPosition;
        arrowImage.transform.rotation = arrowRotation;  
        arrowImage.gameObject.SetActive(true);

        tutorialPopup.SetActive(true);
    }

    public void DisplayTutorialPanel(string title, string message, string buttonText,
                                     GameObject uiObject)
    {
        // pointing at a given gameobject
        Vector3 pointToPointAt = new Vector3Int(0,0,0);
        int arrowAngle = 0;
        // find the center of the gameobject
        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
        if(rectTransform == null)
        {
            Debug.LogError("No rect transform associated to object " + uiObject);
            return;
        }
        // get the center
        Vector3 gameObjectCenter = rectTransform.position;

        // for comparison we cut the screen in 3 and get nine tiles
        int screenFirstLineX = Screen.width / 3; 
        int screenSecondLineX = 2 * screenFirstLineX;
        int screenFirstLineY = Screen.height / 3;
        int screenSecondLineY = 2 * screenFirstLineY;
        Debug.Log("screenFirstLineX = " + screenFirstLineX);
        Debug.Log("ScreenSecondLineX = " + screenSecondLineX);
        Debug.Log("screenFirstLineY = " + screenFirstLineY);
        Debug.Log("ScreenSecondLineY = " + screenSecondLineY);
        Debug.Log("gameObjectCenter = " + gameObjectCenter);

        // now we check on which tile the guiElement is positioned
        if(gameObjectCenter.y < screenFirstLineY)
        {
            // bottom
            // anchor is top of the object
            pointToPointAt.x = gameObjectCenter.x;
            pointToPointAt.y = gameObjectCenter.y + rectTransform.rect.height / 2;
            if(gameObjectCenter.x < screenFirstLineX)
            {
                Debug.Log("bottom left");
                // left
                // arrow point SW
                arrowAngle = 315;
            }
            else if(gameObjectCenter.x < screenSecondLineX)
            {
                Debug.Log("bottom center");
                // center
                // arrow point S
                arrowAngle = 0;
            }
            else
            {
                Debug.Log("bottom right");
                // right
                // arrow point SE
                arrowAngle = 45;
            }
            
        }
        else if(gameObjectCenter.y < screenSecondLineY)
        {
            // middle
            // anchor is on one side
            pointToPointAt.y = gameObjectCenter.y;
            if(gameObjectCenter.x < screenFirstLineX)
            {
                Debug.Log("middle left");
                // left
                // anchor is on the right
                pointToPointAt.x = gameObjectCenter.x + rectTransform.rect.width / 2;
                // arrow points W
                arrowAngle = 270;
            }
            else if(gameObjectCenter.x < screenSecondLineX)
            {
                Debug.Log("middle center");
                // center
                // anchor is on the right as well
                pointToPointAt.x = gameObjectCenter.x + rectTransform.rect.width / 2;
                // arrow points left as well
                arrowAngle = 270;
            }
            else
            {
                Debug.Log("middle right");
                // right
                // anchor is on the left
                pointToPointAt.x = gameObjectCenter.x - rectTransform.rect.width / 2;
                // arrow points right
                arrowAngle = 90;
            }
        }
        else
        {
            // top
            // anchor is on the bottom
            pointToPointAt.x = gameObjectCenter.x;
            pointToPointAt.y = gameObjectCenter.y - rectTransform.rect.height / 2;
            if(gameObjectCenter.x < screenFirstLineX)
            {
                Debug.Log("top left");
                // left
                // arrow points NW
                arrowAngle = 225;
            }
            else if(gameObjectCenter.x < screenSecondLineX)
            {
                Debug.Log("top center");
                // center
                // arrow points N
                arrowAngle = 180;
            }
            else
            {
                Debug.Log("top right");
                // right
                // arrow points NE
                arrowAngle = 135;
            }
            
        }

        Debug.Log("The arrows points at " + pointToPointAt + " with an angle of " +arrowAngle);

        DisplayTutorialPanelAtPoint(title, message, buttonText, pointToPointAt, arrowAngle);

    }

    public void DisplayTutorialPanel(string title, string message, string buttonText,
                                     Location tile)
    {
        Debug.Log("Displayng for tile " + tile);
        
        Vector3 tileCoordinates = camera.WorldToScreenPoint(tilemap.GetCellCenterWorld(new Vector3Int(tile.x, tile.y, 0)));
        tileCoordinates.y = tileCoordinates.y - 64; // this is needed because the sprite of the tile is not centered
        // check where is the tile compared to the screen center
        // finds the center of the screen
        int screenCenterX = Screen.width / 2 ;
        int screenCenterY = Screen.height / 2;
        int arrowAngle = 0;
        if(tileCoordinates.x < screenCenterX && tileCoordinates.y < screenCenterY)
        {
            // bottom left
            // the box goes NE so arrow points SW
            arrowAngle = 315;
        }
        else if(tileCoordinates.x < screenCenterX && tileCoordinates.y >= screenCenterY)
        {
            // top left
            // the box goes SE so arrow points NW
            arrowAngle = 225;
        }
        else if(tileCoordinates.x >= screenCenterX && tileCoordinates.y < screenCenterY)
        {
            // bottom right
            // the box goes NW so arrow points SE
            arrowAngle = 45;
        }
        else if(tileCoordinates.x >= screenCenterX && tileCoordinates.y >= screenCenterY)
        {
            // top right
            // the box goes SW so arrow Point NE
            arrowAngle = 135;
        }


        DisplayTutorialPanelAtPoint(title, message, buttonText, tileCoordinates, arrowAngle );
    }

    public void DisplayTutorialPanel(string title, string message, string buttonText)
    {
        // display a tutorial without an arrow at the center of the screen

        int screenCenterX = Screen.width / 2 ;
        int screenCenterY = Screen.height / 2;
        Vector3 popupPosition = new Vector3(screenCenterX, screenCenterY);

        Text[] texts = tutorialPopup.GetComponentsInChildren<Text>();
        Text tutorialTitle = texts[0];
        Text tutorialMessage = texts[1];
        Text tutorialButtonText = texts[2];
        tutorialTitle.text = title;
        tutorialMessage.text = message;
        tutorialButtonText.text = buttonText;

        tutorialPopup.transform.position = popupPosition;
        arrowImage.gameObject.SetActive(false);

        tutorialPopup.SetActive(true);

    }

    private void DisplayTutorialPanelAtPoint(string title, string message, string buttonText, 
                                Vector3 pointToPointAt, int arrowAngle)
    {
        // display the tutorial popup to point at a given point
        Vector3 arrowLocation = new Vector3(0,0,0);
        Vector3 popupLocation = new Vector3(0,0,0);
        Quaternion arrowRotation = Quaternion.Euler(0,0,0);
        
        Debug.Log("Position: (" + pointToPointAt.x + "," + pointToPointAt.y + ")");
        int arrowWidth = (int)arrowImage.gameObject.GetComponent<RectTransform>().rect.width;
        int arrowHeight = (int)arrowImage.gameObject.GetComponent<RectTransform>().rect.height;
        RectTransform rt = tutorialPopup.GetComponent<RectTransform>();
        float popupWidth = rt.sizeDelta.x * rt.localScale.x;
        float popupHeight = rt.sizeDelta.y * rt.localScale.y;
        Debug.Log("(" + popupWidth + "," + popupHeight +")");

        if(arrowAngle == 0)
        {
            // box N of point, arrow points S
            arrowLocation.x = pointToPointAt.x;
            arrowLocation.y = (int)(pointToPointAt.y + arrowHeight / 2);

            popupLocation.x = arrowLocation.x;
            popupLocation.y = (int)(arrowLocation.y + (popupHeight / 2 + arrowHeight / 2));

        }
        else if(arrowAngle == 45)
        {
            // box NW of the point, arrow points SE
            arrowLocation.x = (int)(pointToPointAt.x - arrowWidth / 2);
            arrowLocation.y = (int)(pointToPointAt.y + arrowHeight / 2);
            
            popupLocation.x = (int)(arrowLocation.x - (popupWidth / 2 + arrowWidth * 0.80));
            popupLocation.y = (int)(arrowLocation.y + (popupWidth / 2 - arrowHeight * 0.10));

        }
        else if(arrowAngle == 90)
        {
            // box W of the point, arrow points E
            arrowLocation.x = (int)(pointToPointAt.x - arrowWidth / 2);
            arrowLocation.y = pointToPointAt.y;

            popupLocation.x = (int)(arrowLocation.x - popupWidth / 2 - arrowWidth * 1.2);
            popupLocation.y = arrowLocation.y;
        } 
        else if(arrowAngle == 135)
        {
            // box SW of the point, arrow points NE
            arrowLocation.x = (int)(pointToPointAt.x - arrowWidth / 2);
            arrowLocation.y = (int)(pointToPointAt.y - arrowHeight / 2);

            popupLocation.x = (int)(arrowLocation.x - (popupWidth / 2 + arrowWidth * 0.80));
            popupLocation.y = (int)(arrowLocation.y - (popupWidth / 2 - arrowHeight * 0.10));
        }
        else if(arrowAngle == 180)
        {
            // box S of the point, arrow points N
            arrowLocation.x = pointToPointAt.x;
            arrowLocation.y = (int)(pointToPointAt.y - arrowHeight / 2);

            popupLocation.x = arrowLocation.x;
            popupLocation.y = (int)(arrowLocation.y - (popupWidth / 2 - arrowHeight /2));
        }
        else if(arrowAngle == 225)
        {
            // box SE of the point, arrow points NW
            // we display the pop-up on the bottom right
            arrowLocation.x = (int)(pointToPointAt.x + arrowWidth / 2);
            arrowLocation.y = (int)(pointToPointAt.y - arrowHeight / 2);

            // popup position is based on the arrow position and popup size
            popupLocation.x = (int)(arrowLocation.x + popupWidth / 2 + arrowWidth * 0.80);
            popupLocation.y = (int)(arrowLocation.y - popupWidth / 2 - arrowHeight * 0.10);
        }
        else if(arrowAngle == 270)
       {
            // box E of the point, arrow points W
            arrowLocation.x = (int)(pointToPointAt.x + arrowHeight / 2);
            arrowLocation.y = pointToPointAt.y;

            popupLocation.x = (int)(arrowLocation.x + (popupWidth / 2 + arrowWidth * 0.80));
            popupLocation.y = arrowLocation.y;

       } 
       else if(arrowAngle == 315)
       {
            // box NE of the point, arrow points SW
            // we display the popup on the top right of the point
            arrowLocation.x = (int)(pointToPointAt.x + arrowWidth / 2);
            arrowLocation.y = (int)(pointToPointAt.y + arrowHeight / 2);

            // popup position is based on the arrow position and popup size
            popupLocation.x = (int)(arrowLocation.x + popupWidth / 2 + arrowWidth * 0.80);
            popupLocation.y = (int)(arrowLocation.y + popupWidth / 2 + arrowHeight * 0.10);
       }

        arrowRotation = Quaternion.Euler(0,0,arrowAngle);
        tutorialPopup.transform.position = popupLocation;  
        arrowImage.transform.position = arrowLocation;
        arrowImage.transform.rotation = arrowRotation;  
        arrowImage.gameObject.SetActive(true);

        Text[] texts = tutorialPopup.GetComponentsInChildren<Text>();
        Text tutorialTitle = texts[0];
        Text tutorialMessage = texts[1];
        Text tutorialButtonText = texts[2];
        tutorialTitle.text = title;
        tutorialMessage.text = message;
        tutorialButtonText.text = buttonText;

        tutorialPopup.SetActive(true);
    }



}