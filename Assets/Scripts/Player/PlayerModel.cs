using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    Rigidbody _rb;
    public float speed;
    bool _isDetectable = true;

    Action _onSpin = delegate { };

    public bool IsDetectable => _isDetectable;

    public Action OnSpin { get => _onSpin; set => _onSpin = value; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 dir)
    {
        dir = dir.normalized;
        dir *= speed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }
    public void Look(Vector3 dir)
    {
        transform.forward = dir;
    }
    public void Spin()
    {
        _isDetectable = !_isDetectable;
        _onSpin?.Invoke();
    }
}
