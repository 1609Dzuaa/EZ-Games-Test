using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class PlayerController : BaseCharacter
{
    [SerializeField] Joystick _joyStick;
    [SerializeField] float _rotationSpeed, _speedFactor;
    [SerializeField] float _radius, _angleIndex;
    [SerializeField] Transform _left, _right;
    [SerializeField] LayerMask _catLayer;
    float _horizontal, _vertical;
    List<int> _listCatsRescued;
    Collider[] _arrCatCols;
    HashSet<Collider> _hashCat;
    bool _isMoving = false;

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
        MeasureSpeed();
    }

    private void FixedUpdate()
    {
        Move();
        TrackCat();
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
        // Tính toán vector tương đối từ tâm đến điểm
        Vector3 relPoint = new Vector3(point.x - center.x, point.z - center.z);

        // Kiểm tra các điều kiện
        return !AreClockwise(sectorStart, relPoint) &&
               AreClockwise(sectorEnd, relPoint) &&
               IsWithinRadius(relPoint, radiusSquared);
    }

    private void MeasureSpeed()
    {
        //Debug.Log("joystick, isM: " + _joyStick.input + ", " + _isMoving);
        if (_joyStick.input != Vector2.zero && !_isMoving)
        {
            _isMoving = true;
            EventsManager.Instance.Notify(EventID.OnMeasureSpeed, _joyStick.input);
            //Debug.Log("noti1");
        }
        else if (_joyStick.input == Vector2.zero && _isMoving)
        {
            _isMoving = false;
            EventsManager.Instance.Notify(EventID.OnMeasureSpeed, _joyStick.input);
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
        for (int i = 0; i < 6; i++)
        {
            float xOffset = Mathf.Sin(i + _angleIndex * Mathf.Rad2Deg) * _radius;
            float zOffset = Mathf.Cos(i + _angleIndex * Mathf.Rad2Deg) * _radius;
            Vector3 newPos = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset);
            Gizmos.DrawLine(transform.position, newPos);
        }
    }
}
