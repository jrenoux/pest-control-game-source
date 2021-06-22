using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TutorialStep
{
    public Vector3 PopupPositionDelta;
    public string Title;
    public string Message;
    public string ButtonText;
    public Vector3 ArrowPositionDelta;
    public Quaternion ArrowRotationDelta;


    // DeltaX and DeltaY are in relation to the left down corner and are in pixels
    // Rolation degrees counterclockwise considering the arrow by default is poiting down
    public TutorialStep(int popupDeltaX, int popupDeltaY, string title, string buttonText, int arrowDeltaX, int arrowDeltaY, int arrowRotation, string message)
    {
        PopupPositionDelta = new Vector3(popupDeltaX, popupDeltaY);
        Title = title;
        Message = message;
        ButtonText = buttonText;
        ArrowPositionDelta = new Vector3(arrowDeltaX, arrowDeltaY);
        ArrowRotationDelta = Quaternion.Euler(0, 0, arrowRotation);
    }
}
