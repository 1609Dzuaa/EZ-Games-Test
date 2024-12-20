using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseCharacter : MonoBehaviour
{
    [SerializeField] protected float _speed = 5.0f;


    protected Animator _anim;
    protected Rigidbody _rb;

    public Animator Anim => _anim;

    public Rigidbody Rb => _rb;

    public float Speed => _speed;

    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
