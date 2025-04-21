using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;

    [SerializeField] GameObject body;

    PlayerInput playerInput;
    public Rigidbody2D _rigidbody { get; private set; }

    PlayerCtrlerModel _model;

    public Vector2 moveVector { get; private set; } = Vector2.zero;
    public Vector2 turnVector { get; private set; } = Vector2.zero;

    PlayerShooter shooter;
    PlayerSlasher slasher;
    PlayerArmerd armerd;

    void Awake()
    {
        TryGetComponent(out playerInput);
        _rigidbody = GetComponent<Rigidbody2D>();

        _model = new PlayerCtrlerModel();
    }

    void Start()
    {
        GameDirector.Instance.finish
            .Subscribe(x => WhenFinishGame())
            .AddTo(this);
    }

    void Update()
    {
        if (GameDirector.Instance.gamestateEnum != GameDirector.GameStateEnum.onGame) return;

        moving();   
    }

    public void ActivateWeapon(int x)
    {
        switch (x)
        {
            case 0:
                body.TryGetComponent(out shooter);
                shooter.enabled = true;

                break;

            case 1:
                body.TryGetComponent(out slasher);
                slasher.enabled = true;

                break;

            case 2:
                body.TryGetComponent(out armerd);
                armerd.enabled = true;

                break;

            default:
                body.TryGetComponent(out shooter);
                shooter.enabled = true;

                break;
        }
    }

    void moving()
    {
        if (_model.isRotatable)
        {
            if(playerInput.actions["Turn"].ReadValue<Vector2>() != Vector2.zero)
            {
                turnVector = playerInput.actions["Turn"].ReadValue<Vector2>();

                body.transform.rotation = Quaternion.Euler(0, 0, Vector2toAngle() - 90);
            }
            else
            {
                Vector2 mousePosi = Camera.main.ScreenToWorldPoint(playerInput.actions["MouseMoved"].ReadValue<Vector2>());

                body.transform.rotation = Quaternion.Euler(0, 0, GetAngle(transform.position, mousePosi) - 90f);
            }
        }

        if (_model.isMovable)
        {
            moveVector = playerInput.actions["Move"].ReadValue<Vector2>();
            _rigidbody.velocity = moveVector * speed;
        }
        else
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

    float Vector2toAngle()
    {
        return Mathf.Atan2(turnVector.y, turnVector.x) * Mathf.Rad2Deg;
    }

    void WhenFinishGame()
    {
        if(shooter != null) shooter.FInishPlaying();
        if(slasher != null) slasher.FInishPlaying();
        if(armerd  != null) armerd.FInishPlaying();
    }

    float GetAngle(Vector2 start, Vector2 terget)
    {
        Vector2 dt = terget - start;

        float degree = Mathf.Atan2(dt.y, dt.x) * Mathf.Rad2Deg;

        return degree;
    }

    public void SetState_move(bool x)
    {
        _model.SetMovable(x);
    }

    public void SetState_rotate(bool x)
    {
        _model.SetRotetable(x);
    }
}
