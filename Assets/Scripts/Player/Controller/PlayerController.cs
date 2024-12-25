using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using static GameConstants;

public class PlayerController : BaseCharacter
{
    [HideInInspector] public Joystick joyStick;
    [SerializeField] float _smoothRotateTime, _upgradeSpeedDuration;
    [SerializeField] float _radius, _angleIndex;
    [SerializeField] Transform _left, _right;
    [SerializeField] LayerMask _catLayer;
    float _horizontal, _vertical, _maxSpeed, _upgradeSpeedTimer, _initialSpeed;
    List<int> _listCatsRescued;
    Collider[] _arrCatCols;
    HashSet<Collider> _hashCat;
    bool _isMoving = false, _hasDebuffed;
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

        EventsManager.Subscribe(EventID.OnDecreaseCat, DecreaseCat);
        EventsManager.Subscribe(EventID.OnUpdatePlayerSpeed, UpdatePlayerSpeed);
        EventsManager.Subscribe(EventID.OnSendJoystick, CacheJoystick);
        EventsManager.Subscribe(EventID.OnStartCount, StartCelebrating);

        //Debug.Log("Sub Joystick");

        #region Init States
        IdleState = new();
        LookBehindState = new();
        CelebrateState = new();
        RunState = new();
        #endregion
        ChangeState(LookBehindState);
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnDecreaseCat, DecreaseCat);
        EventsManager.Unsubscribe(EventID.OnUpdatePlayerSpeed, UpdatePlayerSpeed);
        EventsManager.Unsubscribe(EventID.OnSendJoystick, CacheJoystick);
        EventsManager.Unsubscribe(EventID.OnStartCount, StartCelebrating);
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

    protected override void Update()
    {
        base.Update();
        ReadInput();
        MeasureSpeed();
        DebuffSpeed();
        MeasureMaxPostionX();
        BlockMovement();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //TrackCat();
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
            UIManager.Instance?.TogglePopup(true, EPopupID.Again);
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
        Collider[] arrCols = Physics.OverlapSphere(transform.position, _radius, _catLayer);
        if (arrCols.Length > 0)
            Debug.Log("have cat");
        for (int i = 0; i < arrCols.Length; i++)
        {
        }
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
        //Gizmos.DrawLine(transform.position, _left.position);
        //Gizmos.DrawLine(transform.position, _right.position);
        //Gizmos.DrawWireSphere
        /*for (int i = 0; i < 6; i++)
        {
            float xOffset = Mathf.Sin(i + _angleIndex * Mathf.Rad2Deg) * _radius;
            float zOffset = Mathf.Cos(i + _angleIndex * Mathf.Rad2Deg) * _radius;
            Vector3 newPos = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset);
            Gizmos.DrawLine(transform.position, newPos);
        }*/
    }

    #region Animation Event Related

    private void BackToIdle()
    {
        ChangeState(IdleState);
    }

    #endregion
}
