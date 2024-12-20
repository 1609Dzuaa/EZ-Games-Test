using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

[System.Serializable]
public struct PopupUI
{
    public EPopupID PopupID;
    public GameObject PopupPrefab;
}

public class UIManager : BaseSingleton<UIManager>
{
    [SerializeField] PopupUI[] _popupUIs;
    Dictionary<EPopupID, PopupUI> _dictPopupUIs = new Dictionary<EPopupID, PopupUI>();

    private void Start()
    {
        for (int i = 0; i < _popupUIs.Length; i++)
        {
            _popupUIs[i].PopupPrefab.SetActive(false);
            _dictPopupUIs.Add(_popupUIs[i].PopupID, _popupUIs[i]);
        }
    }

    public void TogglePopup(bool isShow, EPopupID popupID)
    {
        if (_dictPopupUIs.ContainsKey(popupID))
            _dictPopupUIs[popupID].PopupPrefab.SetActive(isShow);
    }
}
