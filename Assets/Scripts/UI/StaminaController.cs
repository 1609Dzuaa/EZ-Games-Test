using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using static GameEnums;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _txtStamina;
    [SerializeField] float _stamina;
    [SerializeField] Slider _sliderStamina;
    [SerializeField] Image _imageSlider;
    float _decreaseEachCount, _initialStamina;

    // Start is called before the first frame update
    void Start()
    {
        _txtStamina.text = _stamina.ToString() + "/" + _stamina.ToString();
        _initialStamina = _stamina;
        _decreaseEachCount = _stamina / 10;
        _sliderStamina.value = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _stamina -= _decreaseEachCount;
                _stamina = Mathf.Clamp(_stamina, 0f, _initialStamina);
                _imageSlider.DOFillAmount(_stamina / _initialStamina, 0.1f);
                _txtStamina.text = _stamina.ToString() + "/" + _initialStamina.ToString();
                Debug.Log("fill");
            }
        }
    }
}
