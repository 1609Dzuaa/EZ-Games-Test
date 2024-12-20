using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEnums
{
    public enum EventID
    {
        OnPlayAgain, //play again khi bị sóng đánh
        OnReceiveResult, //khi kết thúc màn
    }

    public enum EPopupID
    {
        Again,
        Result,

    }

    public enum  EResult
    {
        Failed = 0,
        Completed = 1,
    }
}
