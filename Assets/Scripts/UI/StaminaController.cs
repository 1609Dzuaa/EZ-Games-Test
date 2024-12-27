using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using static GameEnums;
using static GameConstants;
using UnityEngine.UI;

public struct StaminaSpeed
{
    public float StaminaDecrease;
    public float SpeedIncrease;

    public StaminaSpeed(float stamina, float speed)
    {
        StaminaDecrease = stamina;
        SpeedIncrease = speed;
    }
}

public struct SpeedInfor
{
    public float MaxSpeedable;
    public float MaxSpeed;

    public SpeedInfor(float maxSpeedable, float maxSpeed)
    {
        MaxSpeedable = maxSpeedable;
        MaxSpeed = maxSpeed;
    }
}

public class StaminaController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _txtStamina;
    [SerializeField] float _stamina, _tweenDuration;
    [SerializeField] Slider _sliderStamina;
    [SerializeField] Image _imageSlider;
    [SerializeField] Transform _staminaSpawn;
    float _decreasePrefab, _initialStamina, _decreaseStamina, _cacheIncrease, _recoverTimer;
    float _maxSpeedable;
    bool _allowDecrease = true, _phase2Started;
    int _countTouch = 0;
    const float DECREASE_EACH_COUNT_FACTOR = 10.0f;
    const float SPEED_INCREASE_EACH_COUNT_FACTOR = 0.05f;
    const float STAMINA_DECREASE_EACH_COUNT_FACTOR = 0.5f;
    const float MIN_STAMINA_ENABLED = 0f;

    //decrease prefab = InitStamina / 10
    //decrease stamina = decrease prefab / 2
    //speed increase each = decrease stamina / 10
    void Awake()
    {
        _txtStamina.text = _stamina.ToString() + "/" + _stamina.ToString();
        _initialStamina = _stamina;
        _decreasePrefab = _stamina / DECREASE_EACH_COUNT_FACTOR;
        _maxSpeedable = _initialStamina / DECREASE_EACH_COUNT_FACTOR;
        _decreaseStamina = _decreasePrefab * STAMINA_DECREASE_EACH_COUNT_FACTOR;
        _sliderStamina.value = SLIDER_MAX_VALUE;
    }

    private void HideUI(object obj) => gameObject.SetActive(false);

    private void AllowTouchForSpeed(object obj)
    {
        _allowDecrease = true;
        _phase2Started = true;
        _recoverTimer = Time.time;
        //StartCoroutine(RecoverStamina());
    }

    private void Start()
    {
        EventsManager.Subscribe(EventID.OnStartCount, DisplayUI);
        EventsManager.Subscribe(EventID.OnAllowToPlay, StopDecrease);
        EventsManager.Subscribe(EventID.OnReloadLevel, AllowToDecrease);
        EventsManager.Subscribe(EventID.OnStartPhase2, AllowTouchForSpeed);
        EventsManager.Subscribe(EventID.OnReceiveResult, HideUI);

        _cacheIncrease = _decreasePrefab * SPEED_INCREASE_EACH_COUNT_FACTOR;
        StaminaSpeed sp = new StaminaSpeed(_decreasePrefab, _cacheIncrease);
        EventsManager.Notify(EventID.OnUpgradeSpeed, sp);
        gameObject.SetActive(false);
        //Debug.Log("SMC Noti upgrade");
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnStartCount, DisplayUI);
        EventsManager.Unsubscribe(EventID.OnAllowToPlay, StopDecrease);
        EventsManager.Unsubscribe(EventID.OnReloadLevel, AllowToDecrease);
        EventsManager.Unsubscribe(EventID.OnStartPhase2, AllowTouchForSpeed);
        EventsManager.Unsubscribe(EventID.OnReceiveResult, HideUI);
    }

    private void DisplayUI(object obj) => gameObject.SetActive(true);

    private void AllowToDecrease(object obj)
    {
        //reset things here
        _allowDecrease = true;
        _stamina = _initialStamina;
        _txtStamina.text = ((int)_stamina).ToString() + "/" + _initialStamina.ToString();
        _imageSlider.DOFillAmount(DEFAULT_VALUE_ONE, NEAR_ZERO_THRESHOLD_2);
        _countTouch = DEFAULT_VALUE_ZERO_INT;
    }


    private void StopDecrease(object obj)
    {
        _allowDecrease = false;
        EventsManager.Notify(EventID.OnUpdatePlayerSpeed, new SpeedInfor(_maxSpeedable, _cacheIncrease * _countTouch));
        StartCoroutine(RecoverStamina());
        Debug.Log("notify update speed");
    }

    private IEnumerator RecoverStamina()
    {
        float duration = Mathf.Floor((_initialStamina - _stamina) / _decreasePrefab);
        _recoverTimer = Time.time;
        //Debug.Log("need " + duration + "s to finish recover");

        //Debug.Log("current: " + _stamina + ", init: " + _initialStamina);

        while (_stamina < _initialStamina)
        {
            //Debug.Log("inside loop, b4 check");
            HandleRecoverStamina();

            yield return null;
        }
    }

    private void HandleRecoverStamina()
    {
        if (Time.time - _recoverTimer >= A_SECOND)
        {
            _stamina += _decreasePrefab;
            _stamina = Mathf.Min(_stamina, _initialStamina);

            _txtStamina.text = ((int)_stamina).ToString() + "/" + _initialStamina.ToString();
            _imageSlider.DOFillAmount(_stamina / _initialStamina, _tweenDuration);

            _recoverTimer = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && _allowDecrease)
        {
            if (_stamina >= _decreaseStamina)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _stamina -= (_decreasePrefab * STAMINA_DECREASE_EACH_COUNT_FACTOR);
                    _stamina = Mathf.Clamp(_stamina, MIN_STAMINA_ENABLED, _initialStamina);
                    _imageSlider.DOFillAmount(_stamina / _initialStamina, _tweenDuration);
                    _txtStamina.text = _stamina.ToString() + "/" + _initialStamina.ToString();
                    SpawnPrefab(EPoolable.StaminaPrefab, _staminaSpawn.position);
                    SpawnPrefab(EPoolable.SpeedPrefab, touch.position);
                    EventsManager.Notify(EventID.OnIncreaseSpeedUI, _cacheIncrease);
                    if (_phase2Started)
                    {
                        EventsManager.Notify(EventID.OnTouchForSpeed, _cacheIncrease);
                        //HandleRecoverStamina();
                    }
                    _countTouch++;
                }
            }
        }
    }

    private void SpawnPrefab(EPoolable name, Vector3 spawnPosition)
    {
        GameObject prefab = Pool.Instance.GetObjectInPool(name);
        prefab.transform.SetParent(_staminaSpawn);
        prefab.transform.position = spawnPosition;
        prefab.SetActive(true);
        if (name == EPoolable.SpeedPrefab)
        {
            string id = prefab.GetComponent<SpeedPrefab>().PrefabID;
            Speed sp = new Speed(id, _countTouch);
            EventsManager.Notify(EventID.OnIncreaseSpeed, sp);
        }
    }
}
