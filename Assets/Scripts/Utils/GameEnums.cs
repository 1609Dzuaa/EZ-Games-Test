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
        OnMeasureSpeed,
        OnUpgradeSpeed, //upgrade speed khi touch scr lúc countdown
        OnIncreaseSpeed, //tăng speed cho player khi touch x lần (x là tham số truyền vào)
        OnSendPosition, //gửi vị trí của player, wave cho MapSlider
        OnRevive,
        OnUpdatePlayerSpeed, //update speed của player khi đã countdown xong
        OnSendJoystick,
        OnStartCount, //cho phép đếm ngược
        OnCatSendPosition, //mỗi con mèo gửi vị trí lúc bắt đầu game để hiển thị trên map
        OnCatRescued, //cứu đc mèo thì xoá icon mèo đó, giảm stamina, vận tốc
        OnReloadLevel, //reload lại level (thay đổi 1 vài obj)
        OnDiscovered, //mèo bị phát hiện => run
        OnCatOutRange, //mèo out range => reset fill cho mèo
        OnCatBackToPlayer,

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

    public enum EPoolable
    {
        StaminaPrefab,
        SpeedPrefab,

    }

    public enum EPlayerState
    {
        Idle = 0,
        LookBehind = 1,
        Celebrate = 2,
        Run = 3,

    }

    public enum ECatState
    {
        Idle = 0,
        Run = 1,
    }
}
