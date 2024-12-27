using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    [SerializeField] GameObject _measureComponents, _readyController;
    [SerializeField] Image _imageTransition;
    [SerializeField] float _distance, _duration;

    [Header("Delay việc reload để chờ load scene")]
    [SerializeField] float _delayReload;
    Dictionary<EPopupID, PopupUI> _dictPopupUIs = new Dictionary<EPopupID, PopupUI>();
    Vector3 _initialPos;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < _popupUIs.Length; i++)
        {
            _popupUIs[i].PopupPrefab.SetActive(false);
            _dictPopupUIs.Add(_popupUIs[i].PopupID, _popupUIs[i]);
        }
        _initialPos = _imageTransition.transform.localPosition;
        //EventsManager.Subscribe(EventID.OnReceiveResult, HideMeasureComponents);
        //_measureComponents.SetActive(false);
        //_readyController.SetActive(false);
        //EventsManager.Notify(EventID.OnStartCount);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        //EventsManager.Unsubscribe(EventID.OnReceiveResult, HideMeasureComponents);
    }

    private void HideMeasureComponents(object obj) => _measureComponents.SetActive(false);

    private void ShowMeasureComponents(object obj) => _measureComponents.SetActive(true);

    public void TogglePopup(bool isShow, EPopupID popupID)
    {
        if (_dictPopupUIs.ContainsKey(popupID))
        {
            foreach (var popup in _popupUIs)
                popup.PopupPrefab.SetActive(false);
            _dictPopupUIs[popupID].PopupPrefab.SetActive(isShow);
        }
    }

    public void TransitionAndSwitchScene(int sceneIndex)
    {
        float endValue = _imageTransition.transform.localPosition.x - _distance;
        _imageTransition.transform.DOLocalMoveX(endValue, _duration).OnComplete(() =>
        {
            float endValue2 = _imageTransition.transform.localPosition.x - _distance;
            //Debug.Log("Middile: " + _imageTransition.transform.localPosition);
            GameManager.Instance.LoadScene(sceneIndex);
            StartCoroutine(DelayReload());
            //_measureComponents.SetActive(true);
            //_readyController.SetActive(false);

            _imageTransition.transform.DOLocalMoveX(endValue2, _duration).OnComplete(() =>
            {
                _imageTransition.transform.localPosition = _initialPos;
                //_readyController.SetActive(true);
                //EventsManager.Notify(EventID.OnStartCount);
                //Debug.Log("Done");
            });
        });
    }

    private IEnumerator DelayReload()
    {
        yield return new WaitForSeconds(_delayReload);
        EventsManager.Notify(EventID.OnReloadLevel);
    }
}
