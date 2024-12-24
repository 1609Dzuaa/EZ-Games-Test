using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using static GameConstants;

public class PlayerController : BaseCharacter
{
    [HideInInspector] public Joystick joyStick;
    [SerializeField] float _smoothRotateTime;
    [SerializeField] float _radius, _angleIndex;
    [SerializeField] Transform _left, _right;
    [SerializeField] LayerMask _catLayer;
    float _horizontal, _vertical, _maxSpeed;
    List<int> _listCatsRescued;
    Collider[] _arrCatCols;
    HashSet<Collider> _hashCat;
    bool _isMoving = false;
    Vector3 _input;

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
        EventsManager.Instance.Subscribe(EventID.OnDecreaseCat, DecreaseCat);
        EventsManager.Instance.Subscribe(EventID.OnUpdatePlayerSpeed, UpdatePlayerSpeed);
        EventsManager.Instance.Subscribe(EventID.OnSendJoystick, CacheJoystick);
        EventsManager.Instance.Subscribe(EventID.OnStartCount, StartCelebrating);
    }

    private void Start()
    {
        EventsManager.Instance.Notify(EventID.OnSendPosition, new MapSlider
        {
            PlayerPos = transform,
            WavePos = null
        });

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
        EventsManager.Instance.Unsubscribe(EventID.OnDecreaseCat, DecreaseCat);
        EventsManager.Instance.Unsubscribe(EventID.OnUpdatePlayerSpeed, UpdatePlayerSpeed);
        EventsManager.Instance.Unsubscribe(EventID.OnSendJoystick, CacheJoystick);
        EventsManager.Instance.Unsubscribe(EventID.OnStartCount, StartCelebrating);
    }

    private void DecreaseCat(object obj)
    {
        _listCatsRescued.RemoveAt(_listCatsRescued.Count - 1);
    }

    private void UpdatePlayerSpeed(object obj)
    {
        _speed += (float)obj;
        _maxSpeed = _speed; //speed lúc này là cực đại
        //Debug.Log("updateSpeed: " + _speed);
    }

    private void CacheJoystick(object obj) => joyStick = (Joystick)obj;

    private void StartCelebrating(object obj) => ChangeState(CelebrateState);

    protected override void Update()
    {
        base.Update();
        ReadInput();
        MeasureSpeed();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //TrackCat();
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
            EventsManager.Instance.Notify(EventID.OnReceiveResult, new ResultParams
            {
                Result = EResult.Failed,
                Rescued = _listCatsRescued.Count,
                Timer = 0,
                MaxSpeed = _maxSpeed,
                Money = 0
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
            //Check dựa trên bán kính của cái quạt
            if (IsInsideSector(arrCols[i].transform.position, transform.position, _left.position, _right.position, _radius * _radius))
            {
                Debug.Log("Cat in");
            }
        }
    }

    public bool IsInsideSector(Vector3 point, Vector3 center, Vector3 sectorStart, Vector3 sectorEnd, float radiusSquared)
    {
        Vector3 relPoint = new Vector3(point.x - center.x, point.z - center.z);

        return !AreClockwise(sectorStart, relPoint) &&
               AreClockwise(sectorEnd, relPoint) &&
               IsWithinRadius(relPoint, radiusSquared);
    }

    private void MeasureSpeed()
    {
        if (joyStick == null) return;

        //Debug.Log("joystick, isM: " + joyStick.input + ", " + _isMoving);
        if (joyStick.input != Vector2.zero && !_isMoving)
        {
            _isMoving = true;
            EventsManager.Instance.Notify(EventID.OnMeasureSpeed, joyStick.input);
            //Debug.Log("noti1");
        }
        else if (joyStick.input == Vector2.zero && _isMoving)
        {
            _isMoving = false;
            EventsManager.Instance.Notify(EventID.OnMeasureSpeed, joyStick.input);
            //Debug.Log("noti2");
        }
    }

    private bool AreClockwise(Vector3 v1, Vector3 v2)
    {
        return -v1.x * v2.z + v1.z * v2.x > 0;
    }

    private bool IsWithinRadius(Vector3 v, float radiusSquared)
    {
        return v.x * v.x + v.z * v.z <= radiusSquared;
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
