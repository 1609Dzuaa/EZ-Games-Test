using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using static GameConstants;
using System;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.AI;

public struct CatInfor
{
    public CatController Controller;
    public float PositionX;

    public CatInfor(CatController controller, float posX)
    {
        Controller = controller;
        PositionX = posX;
    }
}

public class CatController : BaseCharacter
{
    //nhớ tống 1 vài cái chung chung vào SO
    [SerializeField] float _radius, _patienceDuration, _tweenDuration;
    [SerializeField] int _segment;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Image _patienceFill;
    [SerializeField] GameObject _patienceBar;
    [SerializeField] NavMeshAgent _agent;
    [HideInInspector] public string ID;
    float _initialSpeed, _initialDuration;
    Tween _tweenFill;

    #region States
    public CatIdleState IdleState;
    public CatRunState RunState;
    #endregion

    private void Start()
    {
        _initialSpeed = _speed;
        _initialDuration = _patienceDuration;
        _speed = DEFAULT_VALUE_ZERO;
        _patienceBar.SetActive(false);
        ID = Guid.NewGuid().ToString();
        Anim = GetComponentInChildren<Animator>();
        EventsManager.Notify(EventID.OnCatSendPosition, new CatInfor(this, transform.position.x));
        EventsManager.Subscribe(EventID.OnDiscovered, RunFromPlayer);
        EventsManager.Subscribe(EventID.OnCatOutRange, KillTween);
        IdleState = new CatIdleState();
        RunState = new CatRunState();
        ChangeState(RunState);
        DrawCircleXZ(_segment, _radius);
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnDiscovered, RunFromPlayer);
        EventsManager.Unsubscribe(EventID.OnCatOutRange, KillTween);
    }

    private void RunFromPlayer(object obj)
    {
        if ((string)obj != ID) return;

        _patienceBar.SetActive(true);
        _agent.SetDestination(new Vector3(150f, 0f, 0f));
        _tweenFill = DOTween.To(() => _patienceDuration, x => _patienceDuration = x, 0, _patienceDuration)
            .OnUpdate(() =>
            {
                _patienceFill.DOFillAmount((_initialDuration - _patienceDuration) / _initialDuration, _tweenDuration);
                //Debug.Log("fillWhenFound: " + _patienceFill.fillAmount);
            }).OnComplete(() =>
            {
                //về vs sen
                _patienceBar.SetActive(false);
                EventsManager.Notify(EventID.OnCatRescued, this);
                _agent.ResetPath();
                Debug.Log("Save success cat: " + name);
            });
        ChangeState(RunState);
        _speed = _initialSpeed;
    }

    private void KillTween(object obj)
    {
        if ((string)obj != ID) return;

        _tweenFill.Kill();
        _patienceDuration = _initialDuration;
        //Debug.Log("fillNotFound: " + _patienceFill.fillAmount);
        _patienceFill.DOFillAmount(DEFAULT_VALUE_ZERO, DEFAULT_VALUE_ONE);
        _patienceBar.SetActive(false);
        //Debug.Log("fill: " + _patienceFill.fillAmount);
    }

    protected override void Update()
    {
        base.Update();
        //DrawCircleXZ(_segment, _radius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(WAVE_TAG))
        {
            //EventsManager.Notify(EventID.OnDecreaseCat);
        }
    }

    //https://www.youtube.com/watch?v=DdAfwHYNFOE
    private void DrawCircleXZ(int steps, float radius)
    {
        _lineRenderer.positionCount = steps;
        for (int i = 0; i < steps; i++)
        {
            float circumProgress = (float)i / steps;

            float currentRad = circumProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRad);
            float zScaled = Mathf.Sin(currentRad);

            float x = xScaled * radius;
            float z = zScaled * radius;

            Vector3 currentPos = new Vector3(x, 0f, z);

            _lineRenderer.SetPosition(i, currentPos);
        }
    }
}
