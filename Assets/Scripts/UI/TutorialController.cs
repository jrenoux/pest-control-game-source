using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TutorialController : MonoBehaviour 
{
    [SerializeField]
    private GameObject tutorialIntroduction;
    [SerializeField]
    private GameObject tutorialPopup;    
    [SerializeField]
    private Text tutorialTitle;
    [SerializeField]
    private Text tutorialMessage;
    [SerializeField]
    private Text tutorialButtonText;
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private GameObject gameCover;
    [SerializeField]
    private GameObject finalArrow;
    [SerializeField]
    private GameObject finalArrowDown;  
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject arrow;

    public enum CardinalAngles:int {
        N = 315,
        NW = 0,
        W = 45,
        SW = 90,
        S = 135,
        SE = 180,
        E = 225,
        NE = 270
    }

    public void Start() 
    {
        tutorialIntroduction.SetActive(false);
        tutorialPopup.SetActive(false);
        // hide all arrows
        arrow.gameObject.SetActive(false);

    }

    public void Update()
    {
    
    }

    public void DeactivateTutorialIntroduction()
    {
        tutorialIntroduction.SetActive(false);
        gameCover.SetActive(false);
    }

    public void DeactivateTutorialPopup()
    {
        tutorialPopup.SetActive(false);
    }

    public void NextButtonClicked()
    {
        PestApplication.Instance.tutorialManager.NextTutorial();
    }


    public void DisplayTutorialIntroduction()
    {
        tutorialIntroduction.SetActive(true);
    }

    public void DisplayFinalArrow()
    {
        finalArrow.SetActive(true);
    }

    public void HideFinalArrow()
    {
        finalArrow.SetActive(false);
    }

    public void DisplayFinalArrowDown()
    {
        finalArrowDown.SetActive(true);
    }

    public void HideFinalArrowDown()
    {
        finalArrowDown.SetActive(false);
    }

    public void DisplayTutorialPanel(string title, string message, string buttonText,
                                     GameObject uiObject)
    {
        // pointing at a given gameobject
        Vector3 pointToPointAt = new Vector3Int(0,0,0);
        // popuplocation
        Vector2 popupRelativeLocation = new Vector2(0,0);
        CardinalAngles arrowAngle = CardinalAngles.N;
        // find the center of the gameobject
        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
        if(rectTransform == null)
        {
            Debug.LogError("No rect transform associated to object " + uiObject);
            return;
        }
        // get the center
        Vector3 gameObjectCenter = rectTransform.position;

        // for comparison we cut the screen in 2 * 3 and get 6 tiles (left, right, top, middle, bottom)
        int screenMiddleX = Screen.width / 2;
        int screenFirstLineY = Screen.height / 3;
        int screenSecondLineY = 2 * screenFirstLineY;
        Debug.Log("screenFirstLineX = " + screenMiddleX);
        Debug.Log("screenFirstLineY = " + screenFirstLineY);
        Debug.Log("ScreenSecondLineY = " + screenSecondLineY);
        Debug.Log("gameObjectCenter = " + gameObjectCenter);

        (int objectWidth, int objectHeight) = GetSizeOnScreen(uiObject);
        // now we check on which tile the guiElement is positioned
        if(gameObjectCenter.y < screenFirstLineY)
        {
            // bottom
            // anchor is top of the object
            pointToPointAt.x = gameObjectCenter.x;
            pointToPointAt.y = gameObjectCenter.y + objectHeight / 2;
            if(gameObjectCenter.x < screenMiddleX)
            {
                Debug.Log("bottom left");
                // left
                // arrow point SW
                arrowAngle = CardinalAngles.SW;
                // box is NE
                popupRelativeLocation.x = 1;
                popupRelativeLocation.y = -1;
            }
            else
            {
                Debug.Log("bottom right");
                // right
                // arrow point SE
                arrowAngle = CardinalAngles.SE;
                // box is NW
                popupRelativeLocation.x = -1;
                popupRelativeLocation.y = -1;
            }
            
        }
        else if(gameObjectCenter.y < screenSecondLineY)
        {
            // middle
            // anchor is on one side
            pointToPointAt.y = gameObjectCenter.y;
            if(gameObjectCenter.x < screenMiddleX)
            {
                Debug.Log("middle left");
                // left
                // anchor is on the right
                pointToPointAt.x = gameObjectCenter.x + objectWidth / 2;
                // arrow points W
                arrowAngle = CardinalAngles.W;
                // popup is E
                popupRelativeLocation.x = 1;
                popupRelativeLocation.y = 0;
            }
            else
            {
                Debug.Log("middle right");
                // right
                // anchor is on the left
                pointToPointAt.x = gameObjectCenter.x - objectWidth / 2;
                // arrow points E
                arrowAngle = CardinalAngles.E;
                // popup is W
                popupRelativeLocation.x = -1;
                popupRelativeLocation.y = 0;

            }
        }
        else
        {
            // top
            // anchor is on the bottom
            pointToPointAt.x = gameObjectCenter.x;
            pointToPointAt.y = gameObjectCenter.y - objectHeight / 2;
            if(gameObjectCenter.x < screenMiddleX)
            {
                Debug.Log("top left");
                // left
                // arrow points NW
                arrowAngle = CardinalAngles.NW;
                // popup is SE
                popupRelativeLocation.x = 1;
                popupRelativeLocation.y = 1;
            }
            else
            {
                Debug.Log("top right");
                // right
                // arrow points NE
                arrowAngle = CardinalAngles.NE;
                // popup is SW
                popupRelativeLocation.x = -1;
                popupRelativeLocation.y = 1;
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
        tileCoordinates.y = tileCoordinates.y - 20; // this is needed because the sprite of the tile is not centered
        // check where is the tile compared to the screen center
        // finds the center of the screen
        int screenCenterX = Screen.width / 2 ;
        int screenCenterY = Screen.height / 2;
        CardinalAngles arrowAngle = CardinalAngles.N;
        if(tileCoordinates.x < screenCenterX && tileCoordinates.y < screenCenterY)
        {
            // bottom left
            // the box goes NE so arrow points SW
            arrowAngle = CardinalAngles.SW;
        }
        else if(tileCoordinates.x < screenCenterX && tileCoordinates.y >= screenCenterY)
        {
            // top left
            // the box goes SE so arrow points NW
            arrowAngle = CardinalAngles.NW;
        }
        else if(tileCoordinates.x >= screenCenterX && tileCoordinates.y < screenCenterY)
        {
            // bottom right
            // the box goes NW so arrow points SE
            arrowAngle = CardinalAngles.SE;
        }
        else if(tileCoordinates.x >= screenCenterX && tileCoordinates.y >= screenCenterY)
        {
            // top right
            // the box goes SW so arrow Point NE
            arrowAngle = CardinalAngles.NE;
        }


        DisplayTutorialPanelAtPoint(title, message, buttonText, tileCoordinates, arrowAngle );
    }

    public void DisplayIntroduction()
    {
        tutorialPopup.SetActive(false);
        tutorialIntroduction.SetActive(true);
    }

    public void DisplayTutorialPanel(string title, string message, string buttonText)
    {
        // display a tutorial without an arrow at the center of the screen

        int screenCenterX = Screen.width / 2 ;
        int screenCenterY = Screen.height / 2;
        Vector3 popupPosition = new Vector3(screenCenterX, screenCenterY);

        tutorialTitle.text = title;
        tutorialMessage.text = message;
        tutorialButtonText.text = buttonText;

        tutorialPopup.transform.position = popupPosition;

        tutorialPopup.SetActive(true);
        arrow.SetActive(false);

    }

    private void DisplayTutorialPanelAtPoint(string title, string message, string buttonText, 
                                Vector3 pointToPointAt, CardinalAngles arrowAngle)
    {

        // we first need to put the right text
        tutorialTitle.text = title;
        tutorialMessage.text = message;
        tutorialButtonText.text = buttonText;

        // then to activate
        tutorialPopup.SetActive(true);
        arrow.gameObject.SetActive(true);

        // so that we can get the right size of the popup
        (int popupWidth, int popupHeight) = GetSizeOnScreen(tutorialPopup);
        Debug.Log("popupBox size = (" + popupWidth + "," + popupHeight + ")");
        // and of the arrow image
        (int arrowWidth, int arrowHeight) = GetSizeOnScreen(arrow);
        Debug.Log("arrow size = (" + arrowWidth + "," + arrowHeight + ")");

        // now we can get ready to display the popup at the right place
        Vector3 arrowLocation = new Vector3(0,0,0);
        Vector3 popupLocation = new Vector3(0,0,0);
        Quaternion arrowRotation = Quaternion.Euler(0,0,0);      

        // we check where to place them

        if(arrowAngle == CardinalAngles.S)
        {
            // box N of point, arrow points S
            arrowLocation.x = pointToPointAt.x;
            arrowLocation.y = (int)(pointToPointAt.y + arrowHeight * 0.3);

            popupLocation.x = arrowLocation.x;
            popupLocation.y = (int)(arrowLocation.y + (popupHeight / 2));

        }
        else if(arrowAngle == CardinalAngles.SE)
        {
            // box NW of the point, arrow points SE
            arrowLocation.x = (int)(pointToPointAt.x - arrowWidth * 0.3);
            arrowLocation.y = (int)(pointToPointAt.y + arrowHeight * 0.3);
            
            popupLocation.x = (int)(arrowLocation.x - (popupWidth / 2 ));
            popupLocation.y = (int)(arrowLocation.y + (popupHeight / 2));

        }
        else if(arrowAngle == CardinalAngles.E)
        {
            // box W of the point, arrow points E
            arrowLocation.x = (int)(pointToPointAt.x - arrowWidth * 0.7);
            arrowLocation.y = pointToPointAt.y;

            popupLocation.x = (int)(arrowLocation.x - popupWidth / 2 - arrowWidth * 0.3);
            popupLocation.y = arrowLocation.y;
        } 
        else if(arrowAngle == CardinalAngles.NE)
        {
            // box SW of the point, arrow points NE
            arrowLocation.x = (int)(pointToPointAt.x - arrowWidth * 0.7);
            arrowLocation.y = (int)(pointToPointAt.y - arrowHeight * 0.3 );

            popupLocation.x = (int)(arrowLocation.x - (popupWidth / 2));
            popupLocation.y = (int)(arrowLocation.y - (popupHeight / 2));
        }
        else if(arrowAngle == CardinalAngles.N)
        {
            // box S of the point, arrow points N
            arrowLocation.x = pointToPointAt.x;
            arrowLocation.y = (int)(pointToPointAt.y - arrowHeight / 2);

            popupLocation.x = arrowLocation.x;
            popupLocation.y = (int)(arrowLocation.y - (popupHeight / 2 + arrowHeight * 0.3));
        }
        else if(arrowAngle == CardinalAngles.NW)
        {
            // box SE of the point, arrow points NW
            // we display the pop-up on the bottom right
            arrowLocation.x = (int)(pointToPointAt.x + arrowWidth * 0.7);
            arrowLocation.y = (int)(pointToPointAt.y - arrowHeight * 0.3);

            // popup position is based on the arrow position and popup size
            popupLocation.x = (int)(arrowLocation.x + popupWidth / 2);
            popupLocation.y = (int)(arrowLocation.y - popupHeight / 2);
        }
        else if(arrowAngle == CardinalAngles.W)
       {
            // box E of the point, arrow points W
            arrowLocation.x = (int)(pointToPointAt.x + arrowHeight * 0.7);
            arrowLocation.y = pointToPointAt.y;

            popupLocation.x = (int)(arrowLocation.x + (popupWidth / 2 + arrowWidth * 0.3));
            popupLocation.y = arrowLocation.y;

       } 
       else if(arrowAngle == CardinalAngles.SW)
       {
            // box NE of the point, arrow points SW
            // we display the popup on the top right of the point
            arrowLocation.x = (int)(pointToPointAt.x + arrowWidth * 0.7);
            arrowLocation.y = (int)(pointToPointAt.y + arrowHeight * 0.3);

            // popup position is based on the arrow position and popup size
            popupLocation.x = (int)(arrowLocation.x + popupWidth / 2);
            popupLocation.y = (int)(arrowLocation.y + popupHeight / 2);
       }
         arrowRotation = Quaternion.Euler(0,0,(int)arrowAngle);
        tutorialPopup.transform.position = popupLocation;  
        arrow.transform.position = arrowLocation;
        arrow.transform.rotation = arrowRotation;  

        Debug.Log("Popup is located " + tutorialPopup.transform.position);
    }

    private (int, int) GetSizeOnScreen(GameObject uiObject)
    {
        RectTransform rt = uiObject.GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        Rect r = RectTransformUtility.PixelAdjustRect(rt, canvas);
        
        Debug.Log("Screen size: = (" + Screen.width + "," + Screen.height + ")");
        float scaleFactorWidth = Screen.width / canvas.GetComponent<CanvasScaler>().referenceResolution.x;
        float scaleFactorHeight = Screen.height / canvas.GetComponent<CanvasScaler>().referenceResolution.y;
        Debug.Log("Reference resolution = (" + canvas.GetComponent<CanvasScaler>().referenceResolution.x + "," + canvas.GetComponent<CanvasScaler>().referenceResolution.y + ")");
        Debug.Log("Scale factor =  (" + scaleFactorWidth + "," + scaleFactorHeight + ")");
        float widthOnScreen = r.width * scaleFactorWidth;
        float heightOnScreen = r.height * scaleFactorHeight;

        return ((int)widthOnScreen, (int)heightOnScreen);
    }

    private void SetArrowActive(CardinalAngles arrowAngle)
    {
        switch(arrowAngle) {
            case CardinalAngles.NW:
            arrow.SetActive(true);
            break;
        }
    }

}