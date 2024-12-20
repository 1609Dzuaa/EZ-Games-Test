using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupAgain : PopupController
{
    const int BUTTON_CLOSE = 0;
    const int BUTTON_REVIVE = 1;

    public void ButtonOnClick(int index)
    {
        switch(index)
        {
            case BUTTON_CLOSE:
                //GameManager.Instance.ReloadScene();
                break;
            case BUTTON_REVIVE:
                //Revive();
                break;
        }
    }
}
