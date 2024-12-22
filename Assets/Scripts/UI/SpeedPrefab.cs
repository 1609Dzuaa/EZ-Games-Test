using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using static GameEnums;
using UnityEngine.UI;
using System;

public struct Speed
{
    public string ID;
    public int TouchCount;

    public Speed(string id, int touchCount)
    {
        ID = id;
        TouchCount = touchCount;
    }
}

public class SpeedPrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _txtIncrease;
    [SerializeField] Image _icon;
    [SerializeField] Ease _ease;
    [SerializeField] float _fadeDuration, _moveDistanceX, _moveDistanceY;
    [HideInInspector] public string PrefabID;
    const int FADE_IN = 1, FADE_OUT = 0;
    Vector3 _initialPos;
    bool _isFirstOnEnable = true;
    float _cacheIncrease = 0;

    // Start is called before the first frame update
    void Awake()
    {
        EventsManager.Instance.Subscribe(EventID.OnUpgradeSpeed, CacheBaseIncrease);
        EventsManager.Instance.Subscribe(EventID.OnIncreaseSpeed, IncreaseSpeed);
        PrefabID = Guid.NewGuid().ToString();
    }

    private void OnEnable()
    {
        if (_isFirstOnEnable)
        {
            _isFirstOnEnable = false;
            return;
        }

        _initialPos = transform.position;
        Sequence sequence = DOTween.Sequence();

        sequence.Join(_txtIncrease.DOFade(FADE_IN, _fadeDuration).SetEase(_ease));
        sequence.Join(_icon.DOFade(FADE_IN, _fadeDuration).SetEase(_ease));

        float endValueX = transform.position.x + _moveDistanceX;
        float endValueY = transform.position.y + _moveDistanceY;
        Vector3 endValue = new Vector3(endValueX, endValueY, transform.position.z);
        transform.DOMove(endValue, _fadeDuration);

        sequence.OnComplete(() =>
        {
            //move
            endValue = new(transform.position.x + _moveDistanceX, transform.position.y + _moveDistanceY, transform.position.z);
            transform.DOMove(endValue, _fadeDuration);

            //fade
            _txtIncrease.DOFade(FADE_OUT, _fadeDuration).SetEase(_ease);
            _icon.DOFade(FADE_OUT, _fadeDuration).SetEase(_ease);

            //scale
            StartCoroutine(DelayScale());
        });
    }

    private IEnumerator DelayScale()
    {
        yield return new WaitForSeconds(_fadeDuration / 2.0f);

        transform.DOScale(Vector3.zero, _fadeDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        transform.DOScale(Vector3.one, _fadeDuration);
        transform.position = _initialPos;
    }

    private void CacheBaseIncrease(object obj)
    {
        StaminaSpeed sp = (StaminaSpeed)obj;
        _cacheIncrease = sp.SpeedIncrease;
    }

    private void IncreaseSpeed(object obj)
    {
        Speed speed = (Speed)obj;
        if (PrefabID != speed.ID) return;

        _txtIncrease.text = (_cacheIncrease + _cacheIncrease * speed.TouchCount).ToString();
    }

    private void OnDestroy()
    {
        EventsManager.Instance.Unsubscribe(EventID.OnUpgradeSpeed, CacheBaseIncrease);
        EventsManager.Instance.Subscribe(EventID.OnIncreaseSpeed, IncreaseSpeed);
    }
}
