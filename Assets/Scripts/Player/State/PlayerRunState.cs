using UnityEngine;
using static GameConstants;
using static GameEnums;

public class PlayerRunState : BaseState
{
    PlayerController _pController;
    float currentVelocity;

    override public void Enter(BaseCharacter controller)
    {
        base.Enter(controller);
        _pController = (PlayerController)_controller;
        _pController.Anim.SetInteger(STATE, (int)EPlayerState.Run);
        //Debug.Log("Run");
    }

    override public void Exit()
    {
    }

    override public void Update()
    {
        if (Mathf.Abs(_pController.Horizontal) < JOYSTICK_THRESHOLD_MIN && Mathf.Abs(_pController.Vertical) < JOYSTICK_THRESHOLD_MIN)
            _pController.ChangeState(_pController.IdleState);
    }

    override public void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        //xử lý 2 thứ khi move: di chuyển và quay mặt về hướng di chuyển
        //di chuyển trên trục x và z của map với x là trục di chuyển chính
        //=> trục x lúc này là vertical và z là horizontal
        //và phải đảo dấu trục z để phù hợp.
        _pController.Input = new Vector3(_pController.Vertical, 0f, _pController.Horizontal * REVERSE_AXIS_FACTOR);
        _pController.Input.Normalize();
        _pController.Direction = _pController.transform.TransformDirection(_pController.Input);
        Debug.Log("Input, newPos: " + _pController.Input + ", " + _pController.transform.position + _pController.Input * _pController.Speed * Time.deltaTime);
        _pController.Rb.MovePosition(_pController.transform.position + _pController.Direction * _pController.Speed * Time.deltaTime);

        //Debug.Log("Input: " + _pController.Input);
        //Debug.Log("dir, Input: " + _pController.Direction + "; " + _pController.Input);

        if (_pController.Input.normalized != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(_pController.Horizontal, _pController.Vertical) * Mathf.Rad2Deg;
            targetAngle += 90f;
            var angle = Mathf.SmoothDampAngle(_pController.transform.eulerAngles.y, targetAngle, ref currentVelocity, _pController.RotationSpeed);
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            _pController.transform.rotation = targetRotation;
        }
    }
}
