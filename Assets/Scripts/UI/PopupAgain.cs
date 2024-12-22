using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class PopupAgain : PopupController
{
    const int BUTTON_CLOSE = 0;
    const int BUTTON_REVIVE = 1;

    public void ButtonOnClick(int index)
    {
        switch(index)
        {
            case BUTTON_CLOSE:
                UIManager.Instance.TogglePopup(true, EPopupID.Result);
                break;
            case BUTTON_REVIVE:
                EventsManager.Instance.Notify(EventID.OnRevive);
                break;
        }
    }
}
