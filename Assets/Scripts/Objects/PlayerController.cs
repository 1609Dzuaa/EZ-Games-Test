using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacter
{
    [SerializeField] Joystick _joyStick;
    [SerializeField] float _rotationSpeed;
    float _horizontal, _vertical;

    protected override void Update()
    {
        ReadInput();
        Move();
    }

    private void ReadInput()
    {
        _horizontal = _joyStick.Horizontal;
        _vertical = _joyStick.Vertical;
    }

    private void Move()
    {
        Vector3 input = new Vector3(_horizontal, 0, _vertical);

        if (input.magnitude > 1)
        {
            input = input.normalized;
        }

        Debug.Log("input: " + input);

        Vector3 moveDirection = transform.TransformDirection(input);

        _rb.MovePosition(transform.position + moveDirection * _speed * Time.deltaTime);

        if (input != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }
}
