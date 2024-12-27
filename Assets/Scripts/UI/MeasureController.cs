using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using DG.Tweening;
using TMPro;
using static GameConstants;

public class MeasureController : MonoBehaviour
{
    [SerializeField] float _duration;
    [SerializeField] TextMeshProUGUI _txtMaxSpeed;
    [SerializeField] PlayerController _playerRef;
    bool _inGameplay = false;
    Tween _tweenRotate;
    float _tempSpeed; //biến tạm, 0 ảnh hưởng bên player

    // Start is called before the first frame update
    void Start()
    {
        EventsManager.Subscribe(EventID.OnMeasureSpeed, MeasureSpeed);
        EventsManager.Subscribe(EventID.OnAllowToPlay, AllowUpdate);
        EventsManager.Subscribe(EventID.OnReceiveResult, DenyUpdate);
        EventsManager.Subscribe(EventID.OnIncreaseSpeedUI, DisplaySpeed);
        //_playerRef = GameObject.Find(PLAYER_NAME).GetComponent<PlayerController>(); //lười :v
        //Debug.Log("sub");
    }

    private void DenyUpdate(object obj)
    {
        _inGameplay = false;
        _tempSpeed = DEFAULT_VALUE_ZERO;
    }

    private void DisplaySpeed(object obj)
    {
        if (!_playerRef) _playerRef = GameObject.Find(PLAYER_NAME).GetComponent<PlayerController>(); //lười :v

        if (_tempSpeed < NEAR_ZERO_THRESHOLD && _playerRef) _tempSpeed = _playerRef.Speed;

        _tempSpeed += (float)obj;
        _txtMaxSpeed.text = (_tempSpeed).ToString("0.0");
    }

    private void AllowUpdate(object obj) => _inGameplay = true;

    private void Update()
    {
        if (_inGameplay && _txtMaxSpeed != null && _playerRef)
            _txtMaxSpeed.text = _playerRef.Speed.ToString();
    }

    private void MeasureSpeed(object obj)
    {
        Vector2 input = (Vector2)obj;
        _tweenRotate?.Kill();
        _txtMaxSpeed.text = 31.1f.ToString("0.0");
        //Debug.Log("rotate");
        _tweenRotate = transform.DOLocalRotate(new Vector3(0, 0, (input == Vector2.zero) ? 180f : -180f), _duration, RotateMode.WorldAxisAdd);
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnMeasureSpeed, MeasureSpeed);
        EventsManager.Unsubscribe(EventID.OnIncreaseSpeedUI, DisplaySpeed);
        EventsManager.Unsubscribe(EventID.OnAllowToPlay, AllowUpdate);
        EventsManager.Unsubscribe(EventID.OnReceiveResult, DenyUpdate);
    }
}
