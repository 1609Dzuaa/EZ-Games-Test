using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using static GameEnums;
using UnityEngine.UI;

public class StaminaPrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _txtDecrease, _txtMinus;
    [SerializeField] Image _icon;
    [SerializeField] Ease _ease;
    [SerializeField] float _fadeDuration, _moveDistanceX, _moveDistanceY;
    const int FADE_IN = 1, FADE_OUT = 0;
    Vector3 _initialPos;
    bool _isFirstOnEnable = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        EventsManager.Subscribe(EventID.OnUpgradeSpeed, ShowDecrease);

        if (_isFirstOnEnable)
        {
            _isFirstOnEnable = false;
            return;
        }

        _initialPos = transform.position;
        //fade in 3 thang
        Sequence sequence = DOTween.Sequence();

        sequence.Join(_txtDecrease.DOFade(FADE_IN, _fadeDuration).SetEase(_ease));
        sequence.Join(_txtMinus.DOFade(FADE_IN, _fadeDuration).SetEase(_ease));
        sequence.Join(_icon.DOFade(FADE_IN, _fadeDuration).SetEase(_ease));

        float endValueX = transform.position.x - _moveDistanceX;
        float endValueY = transform.position.y - _moveDistanceY;
        Vector3 endValue = new Vector3(endValueX, endValueY, transform.position.z);

        transform.DOMove(endValue, _fadeDuration);

        sequence.OnComplete(() =>
        {
            //move
            endValue = new(transform.position.x - _moveDistanceX, transform.position.y - _moveDistanceY, transform.position.z);
            transform.DOMove(endValue, _fadeDuration);

            //fade
            _txtDecrease.DOFade(FADE_OUT, _fadeDuration).SetEase(_ease);
            _txtMinus.DOFade(FADE_OUT, _fadeDuration).SetEase(_ease);
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
        EventsManager.Unsubscribe(EventID.OnUpgradeSpeed, ShowDecrease);
        transform.DOScale(Vector3.one, _fadeDuration);
        transform.position = _initialPos;
    }

    private void ShowDecrease(object obj)
    {
        StaminaSpeed sp = (StaminaSpeed)obj;
        _txtDecrease.text = sp.StaminaDecrease.ToString();
    }
}
