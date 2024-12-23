using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using static GameConstants;

public class PlayerCelebrateState : BaseState
{
    PlayerController _pController;

    override public void Enter(BaseCharacter controller)
    {
        base.Enter(controller);
        _pController = (PlayerController)_controller;
        _pController.Rb.velocity = Vector3.zero;
        _pController.Anim.SetInteger(STATE, (int)EPlayerState.Celebrate);
        Debug.Log("Celeb");
    }

    override public void Exit() { }

    override public void Update() { }

    override public void FixedUpdate() { }
}