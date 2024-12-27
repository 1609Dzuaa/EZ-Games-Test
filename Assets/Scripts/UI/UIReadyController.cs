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
    float countdown;
    const int END_VALUE_COUNTDOWN = 0;

    void Start()
    {
        EventsManager.Subscribe(EventID.OnStartCount, StartCountdown);
        gameObject.SetActive(false);
        //Debug.Log("Start ready");
    }

    private void StartCountdown(object obj)
    {
        Countdown();
        //Debug.Log("start c");
    }

    private void Countdown()
    {
        gameObject.SetActive(true);
        countdown = _countDuration;
        Tween tweenScaleText = _txtGuide.transform.DOScale(Vector3.one * _scaleFactor, _scaleDuration).SetLoops(-1, LoopType.Yoyo);
        tweenScaleText.Play();
        DOTween.To(() => countdown, x => countdown = x, END_VALUE_COUNTDOWN, _countDuration).OnUpdate(() =>
        {
            _txtCountdown.text = ((int)countdown).ToString();
            //Debug.Log("timer: " + countdown);
        }).OnComplete(() =>
        {
            _txtGuide.transform.localScale = Vector3.one;
            tweenScaleText.Kill();
            gameObject.SetActive(false);
            EventsManager.Notify(EventID.OnAllowToPlay);
            Debug.Log("playyy");
        });
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnStartCount, StartCountdown);
    }

}
