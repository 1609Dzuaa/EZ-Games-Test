using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using DG.Tweening;
using TMPro;

public class UIReadyController : MonoBehaviour
{
    [SerializeField] float _scaleDuration, _countDuration, _scaleFactor;
    [SerializeField] Ease _ease;
    [SerializeField] TextMeshProUGUI _txtCountdown, _txtGuide;
    int countdown = 3;
    const int END_VALUE_COUNTDOWN = 0;

    void Start()
    {
        EventsManager.Instance.Subscribe(EventID.OnStartCount, StartCountdown);
        Countdown(); //tạm thời để test
    }

    private void StartCountdown(object obj) => Countdown();

    private void Countdown()
    {
        Tween tweenScaleText = _txtGuide.transform.DOScale(Vector3.one * _scaleFactor, _scaleDuration).SetLoops(-1, LoopType.Yoyo);
        tweenScaleText.Play();
        DOTween.To(() => countdown, x => countdown = x, END_VALUE_COUNTDOWN, _countDuration).OnUpdate(() =>
        {
            _txtCountdown.text = countdown.ToString();
            //Debug.Log("timer: " + countdown);
        }).OnComplete(() =>
        {
            _txtGuide.transform.localScale = Vector3.one;
            tweenScaleText.Kill();
            gameObject.SetActive(false);
            EventsManager.Instance.Notify(EventID.OnAllowToPlay);
        });
    }

    private void OnDestroy()
    {
        EventsManager.Instance.Unsubscribe(EventID.OnStartCount, StartCountdown);
    }
    
}
