using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : BaseCharacter
{
    float _timer = 0f;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        Move();
        //base.Update();
    }

    private void Move()
    {
        _rb.MovePosition(transform.position + new Vector3(_speed, 0f, 0f) * Time.deltaTime);
        if (Time.time - _timer > 1f)
        {
            Debug.Log("time: " + Time.time);
            _timer = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You lose");
        }
    }
}
