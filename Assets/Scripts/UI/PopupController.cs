using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static GameEnums;

public class PopupController : MonoBehaviour
{
    [SerializeField] protected float _duration;
    [SerializeField] protected Ease _ease;
    [SerializeField] protected EPopupID _popupID;
    [SerializeField] protected float _distance;
    protected Vector3 _initialPos;

    protected virtual void Awake()
    {
        _initialPos = transform.localPosition;
        //Debug.Log("pos: " + _initialPos);
    }

    protected virtual void OnEnable()
    {
        float target = _initialPos.y - _distance;
        //Debug.Log("target: " + target);
        transform.DOLocalMoveY(target, _duration).SetEase(_ease);
    }

    protected virtual void OnDisable()
    {
        transform.DOLocalMoveY(_initialPos.y, _duration).SetEase(_ease);
        //Debug.Log("dis");
    }
}
