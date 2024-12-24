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
    [SerializeField] GameObject _measureComponents;
    Dictionary<EPopupID, PopupUI> _dictPopupUIs = new Dictionary<EPopupID, PopupUI>();

    protected override void Awake()
    {
        base.Awake();
        EventsManager.Instance.Subscribe(EventID.OnReceiveResult, HideMeasureComponents);
    }

    private void Start()
    {
        for (int i = 0; i < _popupUIs.Length; i++)
        {
            _popupUIs[i].PopupPrefab.SetActive(false);
            _dictPopupUIs.Add(_popupUIs[i].PopupID, _popupUIs[i]);
        }
    }

    private void OnDestroy()
    {
        EventsManager.Instance.Unsubscribe(EventID.OnReceiveResult, HideMeasureComponents);
    }

    private void HideMeasureComponents(object obj) => _measureComponents.SetActive(false);

    public void TogglePopup(bool isShow, EPopupID popupID)
    {
        if (_dictPopupUIs.ContainsKey(popupID))
        {
            foreach (var popup in _popupUIs)
                popup.PopupPrefab.SetActive(false);
            _dictPopupUIs[popupID].PopupPrefab.SetActive(isShow);
        }
    }
}
