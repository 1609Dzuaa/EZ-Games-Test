using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GameConstants;
using static GameEnums;

public class MapSlider
{
    public Transform PlayerPos;
    public Transform WavePos;

    public MapSlider(Transform pT = null, Transform wT = null)
    {
        PlayerPos = pT;
        WavePos = wT;
    }
}

public class SliderMapController : MonoBehaviour
{
    [Header("Tính bằng độ dài trục x của Map")]
    [SerializeField] float _mapLength;

    [Header("Sliders")]
    [SerializeField] Slider _playerSlider, _waveSlider;
    [SerializeField] Slider _catSliderPrefab;
    [SerializeField] GameObject _playerRef;

    Transform _player, _wave;
    float _startLinePositionX;
    List<CatInfor> _listCats;
    Dictionary<CatController, Slider> _dictCatSliders;

    // Start is called before the first frame update
    void Start()
    {
        _waveSlider.value = SLIDER_MIN_VALUE;
        _playerSlider.value = SLIDER_NEAR_ZERO;
        _dictCatSliders = new Dictionary<CatController, Slider>();
        EventsManager.Subscribe(EventID.OnStartCount, DisplayUI);
        EventsManager.Subscribe(EventID.OnSendPosition, CachePosition);
        EventsManager.Subscribe(EventID.OnCatSendPosition, AddCat);
        EventsManager.Subscribe(EventID.OnCatRescued, RemoveCat);
        EventsManager.Subscribe(EventID.OnReceiveResult, HideUI);
        gameObject.SetActive(false);
        //_startLinePositionX = _player.position.x; //là startLine trong design file
    }

    private void HideUI(object obj) => gameObject.SetActive(false);


    private void DisplayUI(object obj) => gameObject.SetActive(true);

    private void CachePosition(object obj)
    {
        MapSlider mapSlider = (MapSlider)obj;
        if (mapSlider.PlayerPos != null)
        {
            _player = mapSlider.PlayerPos;
            _startLinePositionX = _player.position.x;
            //Debug.Log("startLinePositionX: " + _startLinePositionX);
        }
        if (mapSlider.WavePos != null)
        {
            _wave = mapSlider.WavePos;
            //Debug.Log("WaveposX: " + _wave.position.x);
        }
    }

    private void AddCat(object obj)
    {
        if (_listCats == null)
            _listCats = new List<CatInfor>();
        CatInfor catAdded = (CatInfor)obj;
        Slider catIcon = Instantiate(_catSliderPrefab, transform);
        catIcon.transform.position = transform.position;
        catIcon.value = (catAdded.PositionX - _startLinePositionX) / _mapLength;
        _listCats.Add(catAdded);
        _listCats.OrderByDescending(x => x.PositionX);
        EventsManager.Notify(EventID.OnCatDisplayRange, _listCats[_listCats.Count - 1]);
        _dictCatSliders.Add(catAdded.Controller, catIcon);
        //Debug.Log("Cat add: " + catAdded.Controller.gameObject.name);
    }

    private void RemoveCat(object obj)
    {
        CatController catRemoved = (CatController)obj;
        _listCats.Remove(_listCats.Find(x => x.Controller == catRemoved));
        _listCats.OrderByDescending(x => x.PositionX);
        Destroy(_dictCatSliders[catRemoved].gameObject);
        EventsManager.Notify(EventID.OnCatDisplayRange, (_listCats.Count > 0) ? _listCats[_listCats.Count - 1] : null);
        //Debug.Log("remove cat:" + (CatController)obj + "out of Slider Map");
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnStartCount, DisplayUI);
        EventsManager.Unsubscribe(EventID.OnSendPosition, CachePosition);
        EventsManager.Unsubscribe(EventID.OnCatSendPosition, AddCat);
        EventsManager.Unsubscribe(EventID.OnCatRescued, RemoveCat);
        EventsManager.Unsubscribe(EventID.OnReceiveResult, HideUI);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateSliders();
    }

    private void UpdateSliders()
    {
        if (_player == null || _wave == null) return;

        //vì bắt đầu count ở startLine(vị trí ban đầu Player) nên sẽ trừ đi 1 đoạn startLine
        if (_player.position.x > _startLinePositionX + NEAR_ZERO_THRESHOLD)
        {
            //Debug.Log("pX: " + _player.position.x);

            _playerSlider.value = (_player.position.x - _startLinePositionX) / _mapLength;
            //Debug.Log("PT, PL: " + _playerRef.transform.position + ", " + _playerRef.transform.localPosition);
            //Debug.Log("rateP: " + _playerSlider.value);
        }
        if (_wave.position.x >= _startLinePositionX)
        {
            //Debug.Log("rateW: " + _wave.position.x / (_mapLength - _startLinePositionX));
            _waveSlider.value = (_wave.position.x - _startLinePositionX) / _mapLength;
        }
    }
}
