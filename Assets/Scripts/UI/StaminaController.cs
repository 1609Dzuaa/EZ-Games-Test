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

public class StaminaController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _txtStamina;
    [SerializeField] float _stamina, _tweenDuration;
    [SerializeField] Slider _sliderStamina;
    [SerializeField] Image _imageSlider;
    [SerializeField] Transform _staminaSpawn;
    float _decreasePrefab, _initialStamina, _decreaseStamina;
    bool _allowDecrease = true;
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
        _decreaseStamina = _decreasePrefab * STAMINA_DECREASE_EACH_COUNT_FACTOR;
        _sliderStamina.value = SLIDER_MAX_VALUE;
        EventsManager.Instance.Subscribe(EventID.OnAllowToPlay, StopDecrease);
    }

    private void Start()
    {
        StaminaSpeed sp = new StaminaSpeed(_decreasePrefab, (_decreasePrefab * SPEED_INCREASE_EACH_COUNT_FACTOR));
        EventsManager.Instance.Notify(EventID.OnUpgradeSpeed, sp);
    }

    private void OnDestroy()
    {
        EventsManager.Instance.Unsubscribe(EventID.OnAllowToPlay, StopDecrease);
    }

    private void StopDecrease(object obj)
    {
        _allowDecrease = false;
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
            EventsManager.Instance.Notify(EventID.OnIncreaseSpeed, sp);
        }
    }
}
