using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;
using static GameEnums;

public class CatRunState : BaseState
{
    CatController _cController;

    public override void Enter(BaseCharacter controller)
    {
        base.Enter(controller);
        _cController = (CatController)_controller;
        _cController.Anim.SetInteger(STATE, (int)ECatState.Run);
        //Debug.Log("CatRun");
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
