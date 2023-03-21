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

    //Chaque variable Network doivent etre init sinon ERROR
    public NetworkVariable<int> score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Pas le droit d'uitiliser les functions "classiques" => awake ou start pour les variables Network mais celles des varNetwork par override
    public override void OnNetworkSpawn()
    {
        score.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + " " + score.Value.ToString());
        };
    }

    // IsOwner permet de savoir si l'objet est au joueur donc a utilisé pour les inputs

    [Header("Movement")]
    private Vector2 inputZQSD;
    [SerializeField] private float forceMove = 10f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        rb.velocity = inputZQSD * forceMove * Time.fixedDeltaTime;
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

    // FCT TO TEST NETWORKVARIABLE
    public void AddScoreTest()
    {
        Debug.Log(OwnerClientId + ' ' + score.ToString());
        if (IsOwner)
        {
            score.Value++;
        }
    }
    #endregion
}
