using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public enum EntityMoveSituation { Idle, Run };

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] GameObject cameraPrefab;
    public GameObject MyCamera;
    public Transform Turret;
    [SerializeField] Animator animator;


    [Header("Movement")]
    private Vector2 inputZQSD;
    private EntityMoveSituation moveSituation;
    [SerializeField] private float speed = 10f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = Turret.GetChild(0).GetComponent<Animator>();
    }

    private void Start()
    {
        Debug.Log("BOUH avant");
        if (!IsOwner)
        {
            MyCamera.SetActive(false);
            return;
        }
        //MyCamera = Instantiate(cameraPrefab, transform);
        //Debug.Log(MyCamera.name);
        //MyCamera.GetComponent<PlayerCamera>().Player = transform;
    }


    void Update()
    {

    }

    private void FixedUpdate()
    {
        Movement();
    }


    private void Movement()
    {
        Vector2 direction = inputZQSD.normalized;

        rb.velocity = direction * speed;

        if (moveSituation == EntityMoveSituation.Idle && direction.magnitude != 0)
        {
            animator.SetTrigger("Run");
            moveSituation = EntityMoveSituation.Run;
        }

        else if (moveSituation == EntityMoveSituation.Run && direction.magnitude == 0)
        {
            animator.SetTrigger("Idle");
            moveSituation = EntityMoveSituation.Idle;
        }
    }


    #region Handler
    //context.phase = Started performed canceled
    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log(context.phase);
        if (!IsOwner)
        {
            return;
        }
        inputZQSD = context.ReadValue<Vector2>();
    }
    public void LookMouse(InputAction.CallbackContext context)
    {
        if (!IsOwner)
        {
            return;
        }
        Vector2 mousePosition = context.ReadValue<Vector2>();
        Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 target = worldMousePosition - (Vector2)transform.position;

        float angle = 0;

        angle = Mathf.Atan2(target.y, target.x)* Mathf.Rad2Deg;
        Turret.rotation = Quaternion.Euler(0, 0, angle -90);
    }
    #endregion
}
