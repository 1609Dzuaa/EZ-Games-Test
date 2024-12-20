using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static GameEnums;

public struct ResultParams
{
    public EResult Result;
    public int Rescued;
    public int Timer;
    public float MaxSpeed;
    public int Money;
}

public class PopupResult : PopupController
{
    [SerializeField] TextMeshProUGUI _txtResult, _txtRescued, _txtTimer, _txtSpeed, _txtGet;

    protected override void Awake()
    {
        base.Awake();
        EventsManager.Instance.Subcribe(EventID.OnReceiveResult, OnReceiveResult);
    }

    private void OnReceiveResult(object obj)
    {
        ResultParams resultParams = (ResultParams)obj;
        _txtResult.text = resultParams.Result == EResult.Completed ? "Completed!" : "Failed!";
        _txtRescued.text = resultParams.Rescued.ToString() + "/5";
        _txtTimer.text = ConvertTimer(resultParams.Timer);
        _txtSpeed.text = resultParams.MaxSpeed.ToString();
        _txtGet.text = "GET " + ConvertMoney(resultParams.Money);
    }

    private string ConvertTimer(int timer)
    {
        int minutes = timer / 60;
        int seconds = timer % 60;
        TimeSpan time = new TimeSpan(0, minutes, seconds);
        string formattedTime = $"{time:hh\\:mm\\:ss}s";
        return formattedTime;
    }

    private string ConvertMoney(int money)
    {
        string formattedMoney = money.ToString();

        if (money >= 1000)
        {
            formattedMoney = $"{(money / 1000.0):0.##}k";
            return formattedMoney;
        }

        return formattedMoney;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.Unsubcribe(EventID.OnReceiveResult, OnReceiveResult);
    }
}
