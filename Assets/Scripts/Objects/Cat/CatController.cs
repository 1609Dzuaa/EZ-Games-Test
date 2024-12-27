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

public struct CatNavMeshInfor
{
    public string ID;
    public Transform NewPos;

    public CatNavMeshInfor(string id, Transform newPos)
    {
        ID = id;
        NewPos = newPos;
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
    bool _hasSetPath, _followPlayer, _isRescued;
    Tween _tweenFill;
    Transform _newPos;
    BoxCollider _col;

    #region States
    public CatIdleState IdleState;
    public CatRunState RunState;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        EventsManager.Subscribe(EventID.OnCatDisplayRange, DisplayRange);
    }

    private void Start()
    {
        _initialSpeed = _speed;
        _initialDuration = _patienceDuration;
        _speed = DEFAULT_VALUE_ZERO;
        _patienceBar.SetActive(false);
        ID = Guid.NewGuid().ToString();
        Anim = GetComponentInChildren<Animator>();
        _col = GetComponent<BoxCollider>();
        EventsManager.Notify(EventID.OnCatSendPosition, new CatInfor(this, transform.position.x));
        EventsManager.Subscribe(EventID.OnDiscovered, RunFromPlayer);
        EventsManager.Subscribe(EventID.OnCatOutRange, KillTween);
        EventsManager.Subscribe(EventID.OnCatBackToPlayer, MoveToPlayer);
        IdleState = new CatIdleState();
        RunState = new CatRunState();
        ChangeState(RunState);
        DrawCircleXZ(_segment, _radius);
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnDiscovered, RunFromPlayer);
        EventsManager.Unsubscribe(EventID.OnCatOutRange, KillTween);
        EventsManager.Unsubscribe(EventID.OnCatBackToPlayer, MoveToPlayer);
        EventsManager.Unsubscribe(EventID.OnCatDisplayRange, DisplayRange);
    }

    private void RunFromPlayer(object obj)
    {
        if ((string)obj != ID) return;

        if (!_hasSetPath)
        {
            _agent.SetDestination(new Vector3(150f, DEFAULT_VALUE_ZERO, DEFAULT_VALUE_ZERO));
            _hasSetPath = true;
        }

        //Debug.Log("run");
        if (_isRescued) return; //prevent maybe being called again ?????

        _patienceBar.SetActive(true);
        _tweenFill = DOTween.To(() => _patienceDuration, x => _patienceDuration = x, 0, _patienceDuration)
            .OnUpdate(() =>
            {
                _patienceFill.DOFillAmount((_initialDuration - _patienceDuration) / _initialDuration, _tweenDuration);
                //Debug.Log("fillWhenFound: " + _patienceFill.fillAmount);
            }).OnComplete(() =>
            {
                //về vs sen
                //do bắn event trc r mới reset path nên bug
                if (_agent.enabled)
                {
                    _agent.ResetPath();
                    //Debug.Log("Reset Path");
                }
                _isRescued = true;
                _lineRenderer.enabled = false;
                _patienceBar.SetActive(false);
                EventsManager.Notify(EventID.OnCatRescued, this);
                //Debug.Log("Save success cat: " + name);
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

    private void MoveToPlayer(object obj)
    {
        CatNavMeshInfor catInfo = (CatNavMeshInfor)obj;
        if (catInfo.ID != ID) return;

        Vector3 newPosition = catInfo.NewPos.position;
        if (newPosition.y > NEAR_ZERO_THRESHOLD_2)
        {
            _agent.enabled = false;
            ChangeState(IdleState);
            transform.position = newPosition;
            _col.enabled = false;
            //Debug.Log("Cat on top");
        }
        else
        {
            _newPos = catInfo.NewPos;
            _agent.ResetPath();
            _agent.SetDestination(newPosition);
            //Debug.Log("cat back to: " + newPosition);
        }
    }

    private void DisplayRange(object obj)
    {
        if (obj == null) return;
        if (!_lineRenderer) return; //called too soon>????

        CatInfor info = (CatInfor)obj;
        if (ID != info.Controller.ID)
        {
            _lineRenderer.enabled = false;
            return;
        }

        _lineRenderer.enabled = true;
    }

    protected override void Update()
    {
        base.Update();
        FollowPlayer();
        //Debug.Log("pos: " + transform.position);
        //DrawCircleXZ(_segment, _radius);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //_rb.WakeUp();
    }

    private void FollowPlayer()
    {
        if (!_isRescued) return;

        if (!_followPlayer)
        {
            if (!_agent.hasPath)
            {
                _followPlayer = true;
                //Debug.Log("allow follow player");
                //lúc này đã về vị trí đứng trong hàng,
                //cho phép đi theo
            }
            return;
        }

        if (!_agent.enabled) return; //vs mèo on top

        _agent.SetDestination(_newPos.position);
        _agent.stoppingDistance = CAT_STOPPING_DISTANCE;

        //Debug.Log("pathEnd, pos, dis: " + _agent.pathEndPosition + ", " + transform.position + ", " + Vector3.Distance(_agent.pathEndPosition, transform.position));
        if (Vector3.Distance(_agent.pathEndPosition, transform.position) <= CAT_STOPPING_DISTANCE)
            ChangeState(IdleState);
        else
            ChangeState(RunState);

        //distance quá nhỏ thì cho về idle
        //0 thì run
        //path luôn đc set
        //set stop distance để phòng th ở run state mãi do agent ch đến đích
        //Debug.Log("Cat update, path, state: " + _agent.hasPath + ", " + _state);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ENTERsleep, collider: " + _rb.IsSleeping() + ", " + other.gameObject.name);
        if (other.CompareTag(WAVE_TAG))
        {
            Debug.Log("cat destroyed");
            EventsManager.Notify(EventID.OnDecreaseCat, this);
        }
        else if (other.CompareTag("Player"))
        {
            Debug.Log("sen im here");
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
