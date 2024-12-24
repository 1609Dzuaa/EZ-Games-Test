using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameEnums;
using static GameConstants;

public struct ResultParams
{
    public EResult Result;
    public int Rescued;
    public int Timer;
    public float MaxSpeed;
    public float Money;
}

public class PopupResult : PopupController
{
    [SerializeField]
    TextMeshProUGUI _txtResult, _txtRescued, _txtTimer,
        _txtSpeed, _txtGetNormal, _txtGetTriple;
    const int GET_TRIPLE = 0;
    const int GET_NORMAL = 1;

    protected override void Awake()
    {
        base.Awake();
        EventsManager.Instance.Subscribe(EventID.OnReceiveResult, OnReceiveResult);
    }

    private void OnReceiveResult(object obj)
    {
        ResultParams resultParams = (ResultParams)obj;
        _txtResult.text = resultParams.Result == EResult.Completed ? "Completed!" : "Failed!";
        _txtRescued.text = resultParams.Rescued.ToString() + "/5";
        _txtTimer.text = ConvertTimer(resultParams.Timer);
        _txtSpeed.text = $"{resultParams.MaxSpeed:0.0} m/s";
        _txtGetNormal.text = "No Thanks, Get " + ConvertMoney(resultParams.Money);
        _txtGetTriple.text = "GET " + ConvertMoney(resultParams.Money * TRIPLE_REWARD);
    }

    private string ConvertTimer(int timer)
    {
        int minutes = timer / 60;
        int seconds = timer % 60;
        TimeSpan time = new TimeSpan(0, minutes, seconds);
        string formattedTime = $"{time:hh\\:mm\\:ss}s";
        return formattedTime;
    }

    private string ConvertMoney(float money)
    {
        string formattedMoney = $"{money:0.00}";

        if (money >= 1000)
        {
            formattedMoney = $"{(money / 1000.0):0.##}k";
            return formattedMoney;
        }

        return formattedMoney;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.Unsubscribe(EventID.OnReceiveResult, OnReceiveResult);
    }

    public void OnGetClick(int index)
    {
        switch (index)
        {
            case GET_TRIPLE:
                Debug.Log("Get Triple");
                break;
            case GET_NORMAL:
                Debug.Log("Get Normal");
                break;
        }
    }

    public void OnClose()
    {
        GameManager.Instance.ReloadScene();
    }
}
