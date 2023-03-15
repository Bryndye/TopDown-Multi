using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    public Transform MyCamera;
    public Transform Turret;

    //[Header("Health")]
    //public int LifeMax = 100;
    //public int CurrentLife = 100;
    //private bool isInvincible = false;
    //private int invincibleTime = 1;
    //float timerInv = 0;
    //[SerializeField] private TextMeshProUGUI healthText;
    //[SerializeField] private GameObject deathScreen;

    [Header("Movement")]
    private Vector2 inputZQSD;
    [SerializeField] private float forceMove = 10f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //CurrentLife = LifeMax;
    }

    void Update()
    {
        //if (!InCinematic)
        //{
        //    //if (isInvincible)
        //    //{
        //    //    ResetInvincible();
        //    //}
        //}
        //HealthText();
    }

    private void FixedUpdate()
    {
        MovementServerRpc();
    }

    /// <summary>
    /// FUNCTIONS TO MOVE YOUR ASS
    /// </summary>
    [ServerRpc]
    private void MovementServerRpc()
    {
        rb.AddForce(inputZQSD * forceMove * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    /// <summary>
    /// INPUTS VALUES GET
    /// </summary>
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

        //Vector2 relative = Turret.InverseTransformPoint(mousePosition);
        angle = Mathf.Atan2(target.y, target.x)* Mathf.Rad2Deg;
        Turret.rotation = Quaternion.Euler(0, 0, angle -90);
        //Turret.Rotate(0, 0, -angle);
    }

    #endregion



    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.layer == 6)
        //{
        //    TakeDamage();
        //}
    }

    /// <summary>
    /// HEALTH
    /// </summary>
    //private void HealthText()
    //{
    //    healthText.text = ((float)CurrentLife / LifeMax) * 100 + "%";          
    //    healthText.color = (float)CurrentLife / LifeMax > 0.3f? Color.green : Color.red;
    //}

    //public void TakeDamage()
    //{
    //    if (isInvincible)
    //    {
    //        return;
    //    }
    //    if (rb.velocity.magnitude / 1 > 0f)
    //    {
    //        isInvincible = true;
    //        CurrentLife -= 10;
    //        camAnim.SetTrigger("shake");
    //    }

    //    if (CurrentLife <= 0)
    //    {
    //        deathScreen.SetActive(true);
    //    }
    //}


    //private void ResetInvincible()
    //{
    //    timerInv += Time.deltaTime;

    //    if (timerInv > invincibleTime)
    //    {
    //        isInvincible = false;
    //        timerInv = 0;
    //    }
    //}

    //public void Respawn()
    //{
    //    CurrentLife = LifeMax;
    //    deathScreen.SetActive(false);
    //}
}
