using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    Animator _anim;
    Rigidbody _rb;
    PlayerModelExample _model;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _model = GetComponent<PlayerModelExample>();
        _model.OnSpin += OnSpinAnimation;
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
