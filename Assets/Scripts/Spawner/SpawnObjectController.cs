using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;
using static GameEnums;

public class SpawnObjectController : BaseSingleton<SpawnObjectController>
{
    [SerializeField] int _numberOfObjects;
    [Header("Phải đảm bảo lượng prefab luôn > số lượng muốn renew")]
    [SerializeField] int _numObObjectsRenew;
    [SerializeField] float _minX, _maxX, _minZ, _maxZ, _minAngle, _maxAngle;
    [SerializeField] float _maxXPhase1;

    [SerializeField] Transform _min, _max;

    [Header("Default range cho vật thể đầu")]
    [SerializeField] float _defaultMinX, _defaultMaxX;
    GameObject[] _arrPrefabsLoad;
    HashSet<int> _hashObjPrevRound;
    List<int> _listIndexPrevRound;
    bool _isFirstRound = true;
    int _prefabIndex; //random prefab nào sẽ đc chọn
    float _prevObjPosX, _xStep;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        _hashObjPrevRound = new HashSet<int>();
        _listIndexPrevRound = new List<int>();
        _arrPrefabsLoad = Resources.LoadAll<GameObject>(PATH_PREFAB_TO_LOAD);
        EventsManager.Subscribe(EventID.OnReloadLevel, ChangeObjects);

        ChangeObjects();

        //Sẽ require số lượng object muốn làm mới ở vòng sau
        //Check nếu 0 phải vòng sau thì sẽ lấy random NumOfObj trong mảng prefab
        //Trong đó có NumOfObjRenew là làm mới so vs map cũ
        //check nếu index item ngẫu nhiên đc lấy ra từ mảng prefab có tồn tại trong
        //hash không, không thì ok, có thì random cho đến khi nào item đc lấy 0 có index trong hash
        //sau khi đủ renew obj thì random như bthg
        //bằng cách này sẽ đảm bảo luôn có renew cái obj mới
    }

    private void ChangeObjects(object obj = null)
    {
        //reset here
        _xStep = (_max.position.x - _min.position.x) / _numberOfObjects;
        _listIndexPrevRound.Clear();
        _prevObjPosX = DEFAULT_VALUE_ZERO;

        if (_isFirstRound)
        {
            //Debug.Log("1st round: " + _arrPrefabsLoad.Length);
            for (int i = 0; i < _numberOfObjects; i++)
            {
                _prefabIndex = Random.Range(0, _arrPrefabsLoad.Length);
                GameObject go = Instantiate(_arrPrefabsLoad[_prefabIndex]);
                if (_hashObjPrevRound.Count < _numObObjectsRenew && !_hashObjPrevRound.Contains(_prefabIndex))
                    _hashObjPrevRound.Add(_prefabIndex);

                SetPositionAndRotation(ref go);
            }
            _isFirstRound = false;
        }
        else
        {
            //Debug.Log("not 1st round: " + _hashObjPrevRound.Count);
            int num = 0;
            for (int i = 0; i < _numberOfObjects; i++)
            {
                GameObject go = null;
                if (num < _numObObjectsRenew)
                {
                    do
                    {
                        _prefabIndex = Random.Range(0, _arrPrefabsLoad.Length);

                    } while (_hashObjPrevRound.Contains(_prefabIndex));

                    go = Instantiate(_arrPrefabsLoad[_prefabIndex]);
                    _listIndexPrevRound.Add(_prefabIndex);
                    num++;
                }
                else
                {
                    _prefabIndex = Random.Range(0, _arrPrefabsLoad.Length);
                    go = Instantiate(_arrPrefabsLoad[_prefabIndex]);
                    _listIndexPrevRound.Add(_prefabIndex);
                }

                //Debug.Log("Index: " + _prefabIndex);
                SetPositionAndRotation(ref go);
            }
            //update hash cho vòng tới
            _hashObjPrevRound.Clear();
            foreach (var index in _listIndexPrevRound) _hashObjPrevRound.Add(index);
        }
        BakeMesh.Instance.Rebaking();
    }

    private void SetPositionAndRotation(ref GameObject go)
    {
        float randomX = DEFAULT_VALUE_ZERO;
        float randomZ = DEFAULT_VALUE_ZERO;
        float YAngles = Random.Range(_minAngle, _maxAngle);

        //là obj đầu tiên
        if (_prevObjPosX < NEAR_ZERO_THRESHOLD)
        {
            randomX = _min.position.x;
            randomZ = Random.Range(_minZ, _maxZ);
        }
        else
        {
            randomX = _xStep;//Random.Range(0f, _xStep);
            randomZ = Random.Range(_minZ, _maxZ);
        }

        go.transform.position = new Vector3(randomX, DEFAULT_VALUE_ZERO, randomZ);
        go.transform.Rotate(DEFAULT_VALUE_ZERO, YAngles, DEFAULT_VALUE_ZERO);
        go.transform.position = new Vector3(_prevObjPosX + randomX, DEFAULT_VALUE_ZERO, randomZ);
        _prevObjPosX = go.transform.position.x;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventsManager.Unsubscribe(EventID.OnReloadLevel, ChangeObjects);
    }
}
