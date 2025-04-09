using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [SerializeField] GameObject panel_Pause;

    void Start()
    {
        TryGetComponent(out playerInput);
        _rigidbody = GetComponent<Rigidbody2D>();

        body.TryGetComponent(out shooter);
        shooter.enabled = true;

        GameDirector.Instance.onFinish.AddListener(WhenFinishGame);

        _model = new PlayerCtrlerModel();
    }

    void Update()
    {
        if (! GameDirector.Instance.onGame) return;

        moving();   
    }

    void moving()
    {
        if (panel_Pause.activeSelf) return;

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
    }

    float Vector2toAngle()
    {
        return Mathf.Atan2(turnVector.y, turnVector.x) * Mathf.Rad2Deg;
    }

    void OnPause()
    {
        if (panel_Pause.activeSelf) return;
        if (! GameDirector.Instance.onGame) return;

        Time.timeScale *= 0.05f;

        panel_Pause.SetActive(true);
    }

    public void RestartGame()
    {
        if (panel_Pause.activeSelf == false) return;

        Time.timeScale /= 0.05f;

        panel_Pause.SetActive(false);
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
