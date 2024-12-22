using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using static GameEnums;
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
    [SerializeField] float _stamina;
    [SerializeField] Slider _sliderStamina;
    [SerializeField] Image _imageSlider;
    [SerializeField] Transform _staminaSpawn;
    float _decreaseEachCount, _initialStamina;
    bool _allowDecrease = true;
    int _countTouch = 0;

    // Start is called before the first frame update
    void Awake()
    {
        _txtStamina.text = _stamina.ToString() + "/" + _stamina.ToString();
        _initialStamina = _stamina;
        _decreaseEachCount = _stamina / 10.0f;
        _sliderStamina.value = 1f;
        EventsManager.Instance.Subscribe(EventID.OnAllowToPlay, StopDecrease);
    }

    private void Start()
    {
        StaminaSpeed sp = new StaminaSpeed(_decreaseEachCount, (_decreaseEachCount /10.0f) / 2.0f);
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
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _stamina -= (_decreaseEachCount / 2.0f);
                _stamina = Mathf.Clamp(_stamina, 0f, _initialStamina);
                _imageSlider.DOFillAmount(_stamina / _initialStamina, 0.1f);
                _txtStamina.text = _stamina.ToString() + "/" + _initialStamina.ToString();
                SpawnPrefab(EPoolable.StaminaPrefab, _staminaSpawn.position);
                SpawnPrefab(EPoolable.SpeedPrefab, touch.position);
                _countTouch++;
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
