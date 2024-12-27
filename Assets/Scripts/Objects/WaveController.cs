using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using static GameConstants;

public class WaveController : BaseCharacter
{
    [SerializeField] float _speedIncrese;

    float _timer = 0f;
    bool _canMove = true;
    Vector3 _initialPlayerPos;
    BoxCollider _boxCol;

    protected override void Awake()
    {
        base.Awake();
        _boxCol = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        EventsManager.Notify(EventID.OnSendPosition, new MapSlider
        {
            PlayerPos = null,
            WavePos = transform
        });
        EventsManager.Subscribe(EventID.OnReceiveResult, StopMoving);
        EventsManager.Subscribe(EventID.OnRevive, Revive);
        EventsManager.Subscribe(EventID.OnSendPosition, CacheInitialPlayerPos);
        EventsManager.Subscribe(EventID.OnStartPhase2, IncreseWaveSpeed);
    }

    private void IncreseWaveSpeed(object obj) => _speed = _speedIncrese;

    private void StopMoving(object obj)
    {
        _canMove = false;
        _boxCol.enabled = false;

    }

    private void CacheInitialPlayerPos(object obj)
    {
        MapSlider mapSlider = (MapSlider)obj;
        if (mapSlider.PlayerPos != null)
        {
            _initialPlayerPos = mapSlider.PlayerPos.position;
        }
    }

    private void Revive(object obj)
    {
        _canMove = true;
        transform.position = _initialPlayerPos;
        UIManager.Instance?.TogglePopup(false, EPopupID.Again);
        StartCoroutine(DelayEnableBoxCol());
    }

    private IEnumerator DelayEnableBoxCol()
    {
        yield return new WaitForSeconds(1f);
        _boxCol.enabled = true;
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnReceiveResult, StopMoving);
        EventsManager.Unsubscribe(EventID.OnSendPosition, CacheInitialPlayerPos);
        EventsManager.Unsubscribe(EventID.OnRevive, Revive);
        EventsManager.Subscribe(EventID.OnStartPhase2, IncreseWaveSpeed);
    }

    protected override void Update()
    {
        Move();
        //base.Update();
    }

    private void Move()
    {
        if (!_canMove) return;

        _rb.MovePosition(transform.position + new Vector3(_speed, 0f, 0f) * Time.deltaTime);
        if (Time.time - _timer > 1f)
        {
            //Debug.Log("time: " + Time.time);
            _timer = Time.time;
        }
    }
}
