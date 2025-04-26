using System;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    Rigidbody _rb;
    public float speed;

    Action _onSpin = delegate { };

    public bool IsDetectable { get; private set; }

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
    
    public void Spin(bool doSpin)
    {
        IsDetectable = doSpin;
        _onSpin?.Invoke();
    }
}
