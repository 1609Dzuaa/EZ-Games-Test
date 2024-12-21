using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;
using DG.Tweening;

public class MeasureController : MonoBehaviour
{
    [SerializeField] float _duration;

    // Start is called before the first frame update
    void Awake()
    {
        EventsManager.Instance.Subscribe(EventID.OnMeasureSpeed, MeasureSpeed);
        Debug.Log("sub");
    }

    private void MeasureSpeed(object obj)
    {
        Vector2 input = (Vector2)obj;
        Debug.Log("rotate");
        transform.DOLocalRotate(new Vector3(0, 0, (input == Vector2.zero) ? 180f : -180f), _duration, RotateMode.WorldAxisAdd);
    }

    private void OnDestroy()
    {
        EventsManager.Instance.Unsubscribe(EventID.OnMeasureSpeed, MeasureSpeed);
    }
}
