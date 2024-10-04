using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;

    [SerializeField] GameObject body;

    PlayerInput playerInput;

    public Vector2 moveVector { get; private set; } = Vector2.zero;
    public Vector2 turnVector { get; private set; } = Vector2.zero;

    [SerializeField] GameDirector _gameDirector;

    PlayerShooter shooter;
    PlayerSlasher slasher;
    PlayerArmerd armerd;

    bool onPlay = true;
    public bool isMovable;
    public bool isRotatable;
    public bool isMoving = false;

    Vector2 posi_BeforeFlame;

    [SerializeField] float moveLlimit_X;
    [SerializeField] float moveLlimit_Y;

    [SerializeField] GameObject panel_Pause;

    void Start()
    {
        TryGetComponent(out playerInput);

        switch (StartSceneDirector.weapon)
        {
            case StartSceneDirector.weaponEnum.weapon0:

                body.TryGetComponent(out shooter);
                shooter.enabled = true;

                break;

            case StartSceneDirector.weaponEnum.weapon1:

                body.TryGetComponent(out slasher);
                slasher.enabled = true;

                break;

            case StartSceneDirector.weaponEnum.weapon2:

                body.TryGetComponent(out armerd);
                armerd.enabled = true;

                break;

            default:

                body.TryGetComponent(out shooter);
                shooter.enabled = true;

                break;
        }

        _gameDirector.onFinish.AddListener(WhenFinishGame);

        posi_BeforeFlame = transform.position;

        isMovable = true;
        isRotatable = true;
    }

    void Update()
    {
        if (!onPlay) return;

        moving();   

        if((Vector2)transform.position == posi_BeforeFlame)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        posi_BeforeFlame = transform.position;
        posi_BeforeFlame.x = Mathf.Clamp(posi_BeforeFlame.x, -1 * moveLlimit_X, moveLlimit_X);
        posi_BeforeFlame.y = Mathf.Clamp(posi_BeforeFlame.y, -1 * moveLlimit_Y, moveLlimit_Y);

        transform.position = posi_BeforeFlame;
    }

    void moving()
    {
        if (panel_Pause.activeSelf) return;

        if (isRotatable)
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

        if (isMovable)
        {
            moveVector = playerInput.actions["Move"].ReadValue<Vector2>();
            transform.Translate(moveVector * speed * Time.deltaTime);
        }
    }

    float Vector2toAngle()
    {
        return Mathf.Atan2(turnVector.y, turnVector.x) * Mathf.Rad2Deg;
    }

    void OnPause()
    {
        if (panel_Pause.activeSelf) return;
        if (!onPlay) return;

        Time.timeScale *= 0.05f;

        panel_Pause.SetActive(true);

        Debug.Log("dde");
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

        onPlay = false;
    }

    float GetAngle(Vector2 start, Vector2 terget)
    {
        Vector2 dt = terget - start;

        float degree = Mathf.Atan2(dt.y, dt.x) * Mathf.Rad2Deg;

        return degree;
    }
}
