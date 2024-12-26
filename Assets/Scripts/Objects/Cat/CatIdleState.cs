using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using static GameConstants;

public class CatIdleState : BaseState
{
    CatController _cController;

    public override void Enter(BaseCharacter controller)
    {
        base.Enter(controller);
        _cController = (CatController)_controller;
        _cController.Anim.SetInteger(STATE, (int)ECatState.Idle);
        Debug.Log("Cat Idle");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
