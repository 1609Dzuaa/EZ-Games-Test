using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using DG.Tweening;

public class MeasureController : MonoBehaviour
{
    [SerializeField] float _duration;
    Tween _tweenRotate;

    // Start is called before the first frame update
    void Start()
    {
        EventsManager.Subscribe(EventID.OnMeasureSpeed, MeasureSpeed);
        //Debug.Log("sub");
    }

    private void MeasureSpeed(object obj)
    {
        Vector2 input = (Vector2)obj;
        _tweenRotate?.Kill();
        //Debug.Log("rotate");
        _tweenRotate = transform.DOLocalRotate(new Vector3(0, 0, (input == Vector2.zero) ? 180f : -180f), _duration, RotateMode.WorldAxisAdd);
    }

    private void OnDestroy()
    {
        EventsManager.Unsubscribe(EventID.OnMeasureSpeed, MeasureSpeed);
    }
}
