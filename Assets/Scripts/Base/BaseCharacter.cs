using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseCharacter : MonoBehaviour
{
    [SerializeField] protected float _speed = 5.0f;


    protected Animator _anim;
    protected Rigidbody _rb;
    protected BaseState _state;

    public Animator Anim { get => _anim; set => _anim = value; }

    public Rigidbody Rb { get => _rb; set => _rb = value; }

    public float Speed => _speed;

    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    public void ChangeState(BaseState state)
    {
        _state?.Exit();
        _state = state;
        _state.Enter(this);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _state?.Update();
    }

    protected virtual void FixedUpdate()
    {
        _state?.FixedUpdate();
    }
}
