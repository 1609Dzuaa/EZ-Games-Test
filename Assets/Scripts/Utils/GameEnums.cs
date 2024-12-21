using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEnums
{
    public enum EventID
    {
        OnAllowToPlay, //khi cho phép chơi (3 2 1)
        OnDecreaseCat, //giảm số mèo khi bị sóng đánh
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
