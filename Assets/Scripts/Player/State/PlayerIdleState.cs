using UnityEngine;
using static GameConstants;
using static GameEnums;

public class PlayerIdleState : BaseState
{
    PlayerController _pController;

    override public void Enter(BaseCharacter controller)
    {
        base.Enter(controller);
        _pController = (PlayerController)_controller;
        //_pController.Rb.velocity = Vector3.zero;
        _pController.Anim.SetInteger(STATE, (int)EPlayerState.Idle);
        //Debug.Log("Idle");
    }

    override public void Exit()
    {

    }

    override public void Update()
    {
        if (Mathf.Abs(_pController.Horizontal) > JOYSTICK_THRESHOLD_MIN || Mathf.Abs(_pController.Vertical) > JOYSTICK_THRESHOLD_MIN)
        {
            _pController.ChangeState(_pController.RunState);
        }
    }

    override public void FixedUpdate()
    {
        
    }
}
