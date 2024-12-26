using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using static GameConstants;
using Unity.Burst.CompilerServices;

public class PlayerController : BaseCharacter
{
    [HideInInspector] public Joystick joyStick;
    [SerializeField] float _smoothRotateTime, _upgradeSpeedDuration;

    [Header("Fan-shaped Related")]
    [SerializeField] float _radius, _leftAngle, _rightAngle;
    [SerializeField] LineRenderer _lineRenderer;

    [Header("Check Cat Related")]
    [SerializeField] int _steps, _raySteps, _rayCount;
    [SerializeField] LayerMask _catLayer;
    [SerializeField] Transform _rayPos;

    float _horizontal, _vertical, _maxSpeed, _upgradeSpeedTimer, _initialSpeed;
    List<int> _listCatsRescued;
    Collider[] _arrCatCols;
    HashSet<CatController> _hashCatFounded, _hashCatSaved;
    bool _isMoving = false, _hasDebuffed, _foundCat;
    Vector3 _input;
    float _maxPositionX;

    #region States

    public PlayerIdleState IdleState;
    public PlayerLookBehindState LookBehindState;
    public PlayerCelebrateState CelebrateState;
    public PlayerRunState RunState;

    #endregion

    #region Getter, Setter

    public Vector3 Input { get => _input; set => _input = value; }

    public float Horizontal { get => _horizontal; set => _horizontal = value; }

    public float Vertical { get => _vertical; set => _vertical = value; }

    public float SmoothRotateTime { get => _smoothRotateTime; set => _smoothRotateTime = value; }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        _listCatsRescued = new List<int>();
    }

    private void Start()
    {
        EventsManager.Notify(EventID.OnSendPosition, new MapSlider
        {
            PlayerPos = transform,
            WavePos = null
        });
        _arrCatCols = new Collider[10];
        _hashCatFounded = new HashSet<CatController>();
        _hashCatSaved = new HashSet<CatController>();

        EventsManager.Subscribe(EventID.OnDecreaseCat, DecreaseCat);
        EventsManager.Subscribe(EventID.OnUpdatePlayerSpeed, UpdatePlayerSpeed);
        EventsManager.Subscribe(EventID.OnSendJoystick, CacheJoystick);
        EventsManager.Subscribe(EventID.OnStartCount, StartCelebrating);
        EventsManager.Subscribe(EventID.OnCatRescued, HandleRescueCat);

        //Debug.Log("Sub Joystick");

        #region Init States
        IdleState = new();
        LookBehindState = new();
        CelebrateState = new();
        RunState = new();
        #endregion
        ChangeState(LookBehindState);
        DrawFanXZ(_steps, _radius, _leftAngle, _rightAngle);
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnDecreaseCat, DecreaseCat);
        EventsManager.Unsubscribe(EventID.OnUpdatePlayerSpeed, UpdatePlayerSpeed);
        EventsManager.Unsubscribe(EventID.OnSendJoystick, CacheJoystick);
        EventsManager.Unsubscribe(EventID.OnStartCount, StartCelebrating);
        EventsManager.Unsubscribe(EventID.OnCatRescued, HandleRescueCat);
    }

    private void DecreaseCat(object obj)
    {
        _listCatsRescued.RemoveAt(_listCatsRescued.Count - 1);
    }

    private void UpdatePlayerSpeed(object obj)
    {
        _initialSpeed = _speed;
        _speed += (float)obj;
        _maxSpeed = _speed; //speed lúc này là cực đại
        _upgradeSpeedTimer = Time.time;
        //Debug.Log("maxSpeed: " + _maxSpeed);
    }

    private void CacheJoystick(object obj) => joyStick = (Joystick)obj;

    private void StartCelebrating(object obj) => ChangeState(CelebrateState);

    private void HandleRescueCat(object obj)
    {
        CatController catRescued = (CatController)obj;
        _hashCatFounded.Remove(catRescued);
        _hashCatFounded.Add(catRescued);
    }

    protected override void Update()
    {
        base.Update();
        ReadInput();
        MeasureSpeed();
        DebuffSpeed();
        MeasureMaxPostionX();
        BlockMovement();
        TrackCat();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void BlockMovement()
    {
        if (transform.position.z > MAX_WIDTH_OF_MAP)
            transform.position = new Vector3(transform.position.x, transform.position.y, MAX_WIDTH_OF_MAP);
        else if (transform.position.z < MIN_WIDTH_OF_MAP)
            transform.position = new Vector3(transform.position.x, transform.position.y, MIN_WIDTH_OF_MAP);
    }

    private void ReadInput()
    {
        if (joyStick == null) return;

        _horizontal = joyStick.Horizontal;
        _vertical = joyStick.Vertical;
        //Debug.Log("H, V: " + _horizontal + "," + _vertical);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(WAVE_TAG))
        {
            UIManager.Instance.TogglePopup(true, EPopupID.Again);
            //Debug.Log("maxX: " + _maxPositionX);
            EventsManager.Notify(EventID.OnReceiveResult, new ResultParams
            {
                Result = EResult.Failed,
                Rescued = _listCatsRescued.Count,
                Timer = (int)Time.time,
                MaxSpeed = _maxSpeed,
                Money = _listCatsRescued.Count * BOUNTY_EACH_CAT + _maxPositionX * BOUNTY_EACH_METTER
            });
            //Debug.Log("You lose");
        }
    }

    private void TrackCat()
    {
        //bắn multiple ray từ vị trí rayPos để track mèo
        //các ray đc bắn có độ dài rayLength và hướng != nhau
        //na ná đh kim
        //https://www.mathsisfun.com/geometry/unit-circle.html
        //dựa vào sin, cos để tính 

        _foundCat = false;
        for (int i = 0; i < _rayCount; i++)
        {
            //mỗi ray cách nhau 1 angleStep
            float angleStep = (_leftAngle - _rightAngle) / _raySteps;
            float currentAngle = _rightAngle + angleStep * i;
            Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * currentAngle), 0f, Mathf.Cos(Mathf.Deg2Rad * currentAngle));

            if (Physics.Raycast(_rayPos.position, direction, out RaycastHit hit, _radius, _catLayer))
            {
                Debug.DrawRay(_rayPos.position, direction * _radius, Color.green);
                _foundCat = true;
                _lineRenderer.enabled = true;
                CatController catDetected = hit.transform.GetComponent<CatController>();
                if (!_hashCatFounded.Contains(catDetected) && !_hashCatSaved.Contains(catDetected))
                {
                    _hashCatFounded.Add(catDetected);
                    EventsManager.Notify(EventID.OnDiscovered, catDetected.ID);
                    Debug.Log("add cat to hashFound: " + catDetected.name);
                }
            }
            else
            {
                Debug.DrawRay(_rayPos.position, direction * _radius, Color.red);
            }
        }

        if (!_foundCat)
        {
            _lineRenderer.enabled = false;
            foreach (var cat in _hashCatFounded)
            {
                Debug.Log("cat " + cat.name + " out range");
                EventsManager.Notify(EventID.OnCatOutRange, cat.ID);
            }
            _hashCatFounded.Clear();
        }
        //Debug.Log("tracking");
    }

    private void MeasureSpeed()
    {
        if (joyStick == null) return;

        //Debug.Log("joystick, isM: " + joyStick.input + ", " + _isMoving);
        if (joyStick.input != Vector2.zero && !_isMoving)
        {
            _isMoving = true;
            EventsManager.Notify(EventID.OnMeasureSpeed, joyStick.input);
            //Debug.Log("noti1");
        }
        else if (joyStick.input == Vector2.zero && _isMoving)
        {
            _isMoving = false;
            EventsManager.Notify(EventID.OnMeasureSpeed, joyStick.input);
            //Debug.Log("noti2");
        }
    }

    private void DebuffSpeed()
    {
        if (Time.time - _upgradeSpeedTimer >= _upgradeSpeedDuration && !_hasDebuffed)
        {
            _hasDebuffed = true;
            _speed = _initialSpeed;
            //Debug.Log("debuff " + _speed);
        }
    }

    private void MeasureMaxPostionX()
    {
        if (transform.position.x > _maxPositionX)
            _maxPositionX = transform.position.x;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.position, _radius);
    }

    //modify hàm DrawCircle bên cat 1 tí
    private void DrawFanXZ(int steps, float radius, float startAngle, float endAngle)
    {
        _lineRenderer.positionCount = steps + 2;
        float angleRange = Mathf.Deg2Rad * (endAngle - startAngle);

        for (int i = 0; i < steps; i++)
        {
            float progress = (float)i / steps;

            float currentRad = Mathf.Deg2Rad * startAngle + progress * angleRange;

            float x = Mathf.Cos(currentRad) * radius;
            float z = Mathf.Sin(currentRad) * radius;

            _lineRenderer.SetPosition(i + 1, new Vector3(x, 0f, z));
        }

        _lineRenderer.SetPosition(steps + 1, Vector3.zero);
    }

    #region Animation Event Related

    private void BackToIdle()
    {
        ChangeState(IdleState);
    }

    #endregion
}
