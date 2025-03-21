using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    Animator _anim;
    Rigidbody _rb;
    PlayerModel _model;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _model = GetComponent<PlayerModel>();
        _model.OnSpin += OnSpinAnimation;
        OnSpinAnimation();
    }

    private void Update()
    {
        if (_model.IsDetectable)
        {
            _anim.SetFloat("Vel", _rb.velocity.magnitude);
        }
        else
        {
            _anim.SetFloat("Vel", 0);
        }
    }
    void OnSpinAnimation()
    {
        _anim.SetBool("Spin", !_model.IsDetectable);
    }
}
