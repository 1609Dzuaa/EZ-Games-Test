using System.Collections;
using System.Collections.Generic;
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

    Transform _player, _wave;

    [Header("Sliders")]
    [SerializeField] Slider _playerSlider, _waveSlider;
    float _startLinePositionX;

    // Start is called before the first frame update
    void Awake()
    {
        _waveSlider.value = SLIDER_MIN_VALUE;
        _playerSlider.value = SLIDER_NEAR_ZERO;
        EventsManager.Instance.Subscribe(EventID.OnSendPosition, CachePosition);
        //_startLinePositionX = _player.position.x; //là startLine trong design file
    }

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

    private void OnDestroy()
    {
        EventsManager.Instance.Unsubscribe(EventID.OnSendPosition, CachePosition);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSliders();
    }

    private void UpdateSliders()
    {
        //vì bắt đầu count ở startLine(vị trí ban đầu Player) nên sẽ trừ đi 1 đoạn startLine
        if (_player.position.x > _startLinePositionX + NEAR_ZERO_THRESHOLD)
        {
            //Debug.Log("pX: " + _player.position.x);

            _playerSlider.value = (_player.position.x - _startLinePositionX) / _mapLength;
            //Debug.Log("rateP: " + _playerSlider.value);
        }
        if (_wave.position.x >= _startLinePositionX)
        {
            //Debug.Log("rateW: " + _wave.position.x / (_mapLength - _startLinePositionX));
            _waveSlider.value = (_wave.position.x - _startLinePositionX) / _mapLength;
        }
    }
}
