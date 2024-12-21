using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class PlayerController : BaseCharacter
{
    [SerializeField] Joystick _joyStick;
    [SerializeField] float _rotationSpeed, _speedFactor;
    float _horizontal, _vertical;
    List<int> _listCatsRescued;

    #region Charge Speed
    int _touchCount = 0;
    bool _allowToPlay = false;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        _listCatsRescued = new List<int>();
        EventsManager.Instance.Subscribe(EventID.OnDecreaseCat, DecreaseCat);
    }

    private void OnDestroy()
    {
        EventsManager.Instance.Unsubscribe(EventID.OnDecreaseCat, DecreaseCat);
    }

    private void DecreaseCat(object obj)
    {
        _listCatsRescued.RemoveAt(_listCatsRescued.Count - 1);
    }

    protected override void Update()
    {
        ReadTouchCount();
        ReadInput();
        Move();
    }

    private void ReadTouchCount()
    {
        if (!_allowToPlay)
        {
            if (Input.touchCount > 0)
            {
                _touchCount++;
            }
        }
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

        //Debug.Log("input: " + input);

        Vector3 moveDirection = transform.TransformDirection(input);

        _rb.MovePosition(transform.position + moveDirection * _speed * Time.deltaTime);

        if (input != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wave"))
        {
            UIManager.Instance.TogglePopup(true, EPopupID.Again);
            EventsManager.Instance.Notify(EventID.OnReceiveResult, new ResultParams
            {
                Result = EResult.Failed,
                Rescued = _listCatsRescued.Count,
                Timer = 0,
                MaxSpeed = _touchCount * _speedFactor + _speed,
                Money = 0
            });
            //Debug.Log("You lose");
        }
    }
}
