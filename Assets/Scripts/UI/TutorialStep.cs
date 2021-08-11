using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TutorialStep
{
    public Vector3? PopupPositionDelta;
    public string Title;
    public string Message;
    public string ButtonText;
    public Vector3? ArrowPositionDelta;
    public Quaternion? ArrowRotationDelta;
    public GridTile tile; // the tile to point at


    // DeltaX and DeltaY are in relation to the left down corner and are in pixels
    // Rolation degrees counterclockwise considering the arrow by default is poiting down
    // fixed position
    public TutorialStep(int popupDeltaX, int popupDeltaY, string title, string buttonText, int arrowDeltaX, int arrowDeltaY, int arrowRotation, string message)
    {
        PopupPositionDelta = new Vector3(popupDeltaX, popupDeltaY);
        Title = title;
        Message = message;
        ButtonText = buttonText;
        ArrowPositionDelta = new Vector3(arrowDeltaX, arrowDeltaY);
        ArrowRotationDelta = Quaternion.Euler(0, 0, arrowRotation);
    }
    // no arrow
    public TutorialStep(int popupDeltaX, int popupDeltaY, string title,  string buttonText, string message)
    {
        PopupPositionDelta = new Vector3(popupDeltaX, popupDeltaY);
        Title = title;
        Message = message;
        ButtonText = buttonText;
        ArrowPositionDelta = null;
        ArrowRotationDelta = null;
    }

    // arrow pointing on a given tile
    public TutorialStep(GridTile tileToShow, string title, string buttonText, string message)
    {                                                                           
        Title = title;
        Message = message;
        ButtonText = buttonText;
        tile = tileToShow;
        ArrowPositionDelta = null;
        ArrowRotationDelta = null;
        PopupPositionDelta = null;
    }
}
